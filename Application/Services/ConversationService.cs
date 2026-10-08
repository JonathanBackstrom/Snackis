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
            conversation.Participants.Select(p => DisplayName(p.User)).ToList(),
            messages.Select(m => m.ToDto()).ToList());
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
}
