namespace Snackis.Application.DTOs;

public record DiscussionDetailDto(
    int Id,
    string Title,
    string AuthorName,
    DateTime CreatedAt,
    bool IsLocked,
    int CategoryId,
    string CategoryName,
    IReadOnlyList<PostDto> Posts
);
