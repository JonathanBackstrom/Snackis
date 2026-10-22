namespace Snackis.Application.DTOs;

public record ConversationListDto(
    int Id,
    string Title,
    bool IsGroup,
    bool IsOwner,
    string? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount);
