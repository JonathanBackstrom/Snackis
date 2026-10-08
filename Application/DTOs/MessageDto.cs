namespace Snackis.Application.DTOs;

public record MessageDto(
    int Id,
    string Body,
    string SenderId,
    string SenderName,
    string? SenderAvatarPath,
    DateTime SentAt);
