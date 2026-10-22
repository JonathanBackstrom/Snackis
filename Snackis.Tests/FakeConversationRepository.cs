using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakeConversationRepository : IConversationRepository
{
    private readonly List<Conversation> conversations = [];
    private readonly List<Message> messages = [];
    private int nextConversationId = 1;
    private int nextMessageId = 1;

    public FakeConversationRepository(params Conversation[] seed)
    {
        foreach (var conversation in seed)
        {
            LaggKonversation(conversation);
        }
    }

    public IReadOnlyList<Message> Meddelanden => messages;

    public FakeConversationRepository MedMeddelanden(params Message[] seed)
    {
        foreach (var message in seed)
        {
            LaggMeddelande(message);
        }

        return this;
    }

    public Task<Conversation?> GetByIdAsync(int id)
        => Task.FromResult(conversations.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<Conversation>> GetForUserAsync(string userId)
        => Task.FromResult<IReadOnlyList<Conversation>>(conversations
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .ToList());

    public Task<Conversation?> FindDirectAsync(string userIdA, string userIdB)
        => Task.FromResult(conversations.FirstOrDefault(c =>
            !c.IsGroup
            && c.Participants.Any(p => p.UserId == userIdA)
            && c.Participants.Any(p => p.UserId == userIdB)));

    public Task<Conversation> AddAsync(Conversation conversation)
    {
        LaggKonversation(conversation);
        return Task.FromResult(conversation);
    }

    public Task<IReadOnlyList<Message>> GetMessagesAsync(int conversationId)
        => Task.FromResult<IReadOnlyList<Message>>(messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToList());

    public Task<Message> AddMessageAsync(Message message)
    {
        LaggMeddelande(message);
        return Task.FromResult(message);
    }

    public Task<Message?> GetMessageAsync(int messageId)
        => Task.FromResult(messages.FirstOrDefault(m => m.Id == messageId));

    public Task DeleteMessageAsync(int messageId)
    {
        var message = messages.FirstOrDefault(m => m.Id == messageId);

        if (message is not null)
        {
            messages.Remove(message);
        }

        return Task.CompletedTask;
    }

    public Task MarkAsReadAsync(int conversationId, string userId) => Task.CompletedTask;

    public Task HideAsync(int conversationId, string userId)
    {
        var conversation = conversations.FirstOrDefault(c => c.Id == conversationId);
        var participant = conversation?.Participants.FirstOrDefault(p => p.UserId == userId);

        if (participant is not null)
        {
            participant.HiddenAt = DateTime.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task AddParticipantAsync(ConversationParticipant participant)
    {
        var conversation = conversations.FirstOrDefault(c => c.Id == participant.ConversationId);
        conversation?.Participants.Add(participant);

        return Task.CompletedTask;
    }

    public Task RemoveParticipantAsync(int conversationId, string userId)
    {
        var conversation = conversations.FirstOrDefault(c => c.Id == conversationId);
        var participant = conversation?.Participants.FirstOrDefault(p => p.UserId == userId);

        if (conversation is not null && participant is not null)
        {
            conversation.Participants.Remove(participant);
        }

        return Task.CompletedTask;
    }

    private void LaggKonversation(Conversation conversation)
    {
        if (conversation.Id == 0)
        {
            conversation.Id = nextConversationId++;
        }
        else if (conversation.Id >= nextConversationId)
        {
            nextConversationId = conversation.Id + 1;
        }

        conversations.Add(conversation);
    }

    private void LaggMeddelande(Message message)
    {
        if (message.Id == 0)
        {
            message.Id = nextMessageId++;
        }
        else if (message.Id >= nextMessageId)
        {
            nextMessageId = message.Id + 1;
        }

        messages.Add(message);
    }
}
