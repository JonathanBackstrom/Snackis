using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IConversationRepository
{
    Task<IReadOnlyList<Conversation>> GetForUserAsync(string userId);
    Task<Conversation?> GetByIdAsync(int id);
    Task<Conversation?> FindDirectAsync(string userIdA, string userIdB);
    Task<Conversation> AddAsync(Conversation conversation);
    Task<IReadOnlyList<Message>> GetMessagesAsync(int conversationId);
    Task<Message> AddMessageAsync(Message message);
    Task MarkAsReadAsync(int conversationId, string userId);
}
