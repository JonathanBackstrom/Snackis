using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IConversationService
{
    Task<IReadOnlyList<ConversationListDto>> GetInboxAsync(string userId);
    Task<ConversationDetailDto> GetAsync(int conversationId, string userId);
    Task<int> StartDirectAsync(string userId, string otherUserId);
    Task SendAsync(SendMessageDto dto, string senderId);
    Task<int> CreateGroupAsync(CreateGroupDto dto, string ownerId);
    Task<IReadOnlyList<ParticipantDto>> SearchInviteCandidatesAsync(int conversationId, string userId, string term);
    Task InviteAsync(int conversationId, string userId, string invitedUserId);
    Task RemoveParticipantAsync(int conversationId, string ownerId, string userIdToRemove);
    Task<int> CreateGroupFromDirectAsync(int conversationId, string userId, CreateGroupDto dto, string invitedUserId);
}
