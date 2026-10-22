using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class ConversationService(IConversationRepository conversationRepo, IUserRepository userRepo) : IConversationService
{
    public async Task<ConversationDetailDto> GetAsync(int conversationId, string userId)
    {
        var conversation = await GetForParticipantAsync(conversationId, userId);

        var messages = await conversationRepo.GetMessagesAsync(conversationId);

        await conversationRepo.MarkAsReadAsync(conversationId, userId);

        return new ConversationDetailDto(
            conversation.Id,
            BuildTitle(conversation, userId),
            conversation.IsGroup,
            conversation.OwnerId,
            conversation.Participants
                .Select(p => new ParticipantDto(p.UserId, DisplayName(p.User)))
                .ToList(),
            messages
                .Select(m => m.ToDto()).ToList());
    }

    private static string DisplayName(ApplicationUser? user)
        => user?.DisplayName ?? user?.UserName ?? "Okänd";

    private async Task<Conversation> GetForParticipantAsync(int conversationId, string userId)
    {
        var conversation = await conversationRepo.GetByIdAsync(conversationId)
            ?? throw new NotFoundException("Konversationen hittades inte.");

        if (!conversation.Participants.Any(p => p.UserId == userId))
            throw new NotFoundException("Konversationen hittades inte.");

        return conversation;
    }

    public async Task<IReadOnlyList<ConversationListDto>> GetInboxAsync(string userId)
    {
        var conversations = await conversationRepo.GetForUserAsync(userId);

        return conversations
            .Select(c =>
            {
                var last = c.Messages
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefault();

             return new ConversationListDto(
                 c.Id,
                 BuildTitle(c, userId),
                 c.IsGroup,
                 c.OwnerId == userId,
                 last?.Body,
                 last?.SentAt,
                 CountUnread(c, userId));

            })
            .OrderByDescending(c => c.LastMessageAt ?? DateTime.MinValue)
            .ToList();
    }

    private static int CountUnread(Conversation conversation, string userId)
    {
        var me = conversation.Participants.FirstOrDefault(p => p.UserId == userId);

        if (me is null)
            return 0;

        return conversation.Messages.Count(m =>
            m.SenderId != userId
            && (me.LastReadAt is null || m.SentAt > me.LastReadAt));
    }

    private string BuildTitle(Conversation conversation, string userId) =>
        conversation.IsGroup
        ? conversation.Name ?? "Grupp"
        : DisplayName(conversation.Participants.FirstOrDefault(p => p.UserId != userId)?.User);
    

    public async Task SendAsync(SendMessageDto dto, string senderId)
    {
        _ = await GetForParticipantAsync(dto.ConversationId, senderId);

        await conversationRepo.AddMessageAsync(new Message
        {
            ConversationId = dto.ConversationId,
            SenderId = senderId,
            Body = dto.Body
        });
    }

    public async Task DeleteMessageAsync(int conversationId, int messageId, string userId)
    {
        _ = await GetForParticipantAsync(conversationId, userId);

        var message = await conversationRepo.GetMessageAsync(messageId)
            ?? throw new NotFoundException("Meddelandet hittades inte");

        if (message.ConversationId != conversationId)
            throw new NotFoundException("Meddelandet hittades inte");

        if (message.SenderId != userId)
            throw new BusinessRuleException("Du kan bara ta bort dina egna meddelanden");

        await conversationRepo.DeleteMessageAsync(messageId);
    }

    public async Task<int> StartDirectAsync(string userId, string otherUserId)
    {
        if (userId == otherUserId)
            throw new BusinessRuleException("Du kan inte skicka meddelanden till dig själv.");

        _ = await userRepo.GetByIdAsync(otherUserId)
            ?? throw new NotFoundException("Användaren hittades inte.");

        var existing = await conversationRepo.FindDirectAsync(userId, otherUserId);

        if (existing is not null)
            return existing.Id;

        var conversation = new Conversation
        {
            IsGroup = false,
            OwnerId = userId,
            Participants =
            [
                new ConversationParticipant { UserId = userId },
                new ConversationParticipant { UserId = otherUserId }
            ]
        };

        var created = await conversationRepo.AddAsync(conversation);
        return created.Id;
    }

    public async Task<int> CreateGroupAsync(CreateGroupDto dto, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BusinessRuleException("Gruppen måste ha ett namn");

        var conversation = new Conversation
        {
            IsGroup = true,
            Name = dto.Name.Trim(),
            OwnerId = ownerId,
            Participants = [new ConversationParticipant { UserId = ownerId}]
        };

        var created = await conversationRepo.AddAsync(conversation);
        return created.Id;
    }

    public async Task<IReadOnlyList<ParticipantDto>> SearchInviteCandidatesAsync(int conversationId, string userId, string term)
    {
        var conversation = await GetForParticipantAsync(conversationId, userId);

        if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
            return [];

        var search = await userRepo.SearchByNameAsync(term.Trim());

        var existing = conversation.Participants
            .Select(p => p.UserId)
            .ToHashSet();

        return search
            .Where(u => !existing.Contains(u.Id))
            .Select(u => new ParticipantDto(u.Id, DisplayName(u)))
            .ToList();
    }

    public async Task InviteAsync(int conversationId, string userId, string invitedUserId)
    {
        var conversation = await GetForParticipantAsync(conversationId, userId);

        if (!conversation.IsGroup)
            throw new BusinessRuleException("Du kan bara bjuda in till gruppkonversationer");

        _ = await userRepo.GetByIdAsync(invitedUserId)
            ?? throw new NotFoundException("Användaren hittades inte");

        if (conversation.Participants.Any(p => p.UserId == invitedUserId))
            throw new BusinessRuleException("Personen är redan med i gruppen");

        await conversationRepo.AddParticipantAsync(new ConversationParticipant
        {
            ConversationId = conversationId,
            UserId = invitedUserId
        });
    }

    public async Task HideAsync(int conversationId, string userId)
    {
        _ = await GetForParticipantAsync(conversationId, userId);

        await conversationRepo.HideAsync(conversationId, userId);
    }

    public async Task LeaveGroupAsync(int conversationId, string userId)
    {
        var conversation = await GetForParticipantAsync(conversationId, userId);

        if (!conversation.IsGroup)
            throw new BusinessRuleException("Du kan bara lämna grupper.");

        if (conversation.OwnerId == userId)
            throw new BusinessRuleException("Som skapare kan du inte lämna gruppen.");

        await conversationRepo.RemoveParticipantAsync(conversationId, userId);
    }

    public async Task RemoveParticipantAsync(int conversationId, string ownerId, string userIdToRemove)
    {
        var conversation = await GetForParticipantAsync(conversationId, ownerId);

        if (!conversation.IsGroup)
            throw new BusinessRuleException("Deltagare kan bara tas bort ur grupper.");

        if (conversation.OwnerId != ownerId)
            throw new BusinessRuleException("Bara den som skapade gruppen kan ta bort deltagare.");

        if (userIdToRemove == conversation.OwnerId)
            throw new BusinessRuleException("Ägaren kan inte tas bort ur gruppen.");

        if (!conversation.Participants.Any(p => p.UserId == userIdToRemove))
            throw new NotFoundException("Personen är inte med i gruppen.");

        await conversationRepo.RemoveParticipantAsync(conversationId, userIdToRemove);
    }

    public async Task<int> CreateGroupFromDirectAsync(int conversationId, string userId, CreateGroupDto dto, string invitedUserId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BusinessRuleException("Gruppen måste ha ett namn");

        var conversation = await GetForParticipantAsync(conversationId, userId);

        if (conversation.IsGroup)
            throw new BusinessRuleException("Konversationen är redan en grupp");

        _ = await userRepo.GetByIdAsync(invitedUserId)
            ?? throw new NotFoundException("Användaren hittades inte");

        var participant = conversation.Participants
            .Select(p => p.UserId)
            .ToHashSet();

        if (participant.Contains(invitedUserId))
            throw new BusinessRuleException("Användaren är redan med i gruppen");

        participant.Add(invitedUserId);

        var group = new Conversation
        {
            IsGroup = true,
            Name = dto.Name.Trim(),
            OwnerId = userId,
            Participants = [.. participant.Select(id => new ConversationParticipant { UserId = id})]
        };

        var created = await conversationRepo.AddAsync(group);

        return created.Id;
    }
}
