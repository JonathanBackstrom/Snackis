using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IConversationService
{
    Task<IReadOnlyList<ConversationListDto>> GetInboxAsync(string userId);
    Task<ConversationDetailDto> GetAsync(int conversationId, string userId);
    Task<int> StartDirectAsync(string userId, string otherUserId);
    Task SendAsync(SendMessageDto dto, string senderId);
}
