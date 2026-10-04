namespace Snackis.Application.DTOs;

public record DiscussionListDto(
    int Id,
    string Title,
    string AuthorName,
    DateTime CreatedAt,
    bool IsLocked
);
