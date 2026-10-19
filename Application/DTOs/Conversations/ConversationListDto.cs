namespace Snackis.Application.DTOs;

public record ConversationListDto(
    int Id,
    string Title,
    bool IsGroup,
    string? LastMessage,
    DateTime? LastMessageAt,
    int UnreadCount);
