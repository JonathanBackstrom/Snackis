namespace Snackis.Application.DTOs;

public record PostDto(
    int Id,
    string Body,
    string AuthorId,
    string AuthorName,
    string? AvatarPath,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsDeleted,
    IReadOnlyList<PostDto> Replies
);
