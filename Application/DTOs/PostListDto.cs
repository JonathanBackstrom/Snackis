namespace Snackis.Application.DTOs;

public record PostListDto
(
    int Id,
    int? ParentPostId,
    string Body,
    string AuthorName,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsDeleted
    );
