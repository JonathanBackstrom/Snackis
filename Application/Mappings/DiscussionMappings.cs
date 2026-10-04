using Snackis.Application.DTOs;
using Snackis.Domain.Entities;

namespace Snackis.Application.Mappings;

public static class DiscussionMappings
{
    public static DiscussionListDto ToListDto(this Discussion discussion)
        => new(
            discussion.Id,
            discussion.Title,
            discussion.Author.DisplayName ?? discussion.Author.UserName ?? "Okänd",
            discussion.CreatedAt,
            discussion.IsLocked);

    public static DiscussionDetailDto ToDetailDto(this Discussion discussion, IReadOnlyList<PostDto> posts)
        => new(
            discussion.Id,
            discussion.Title,
            discussion.Author.DisplayName ?? discussion.Author.UserName ?? "Okänd",
            discussion.CreatedAt,
            discussion.IsLocked,
            discussion.CategoryId,
            discussion.Category.Name,
            posts);

    public static Discussion ToEntity(this CreateDiscussionDto dto, string authorId)
        => new()
        {
            Title = dto.Title,
            CategoryId = dto.CategoryId,
            AuthorId = authorId
        };

    public static Post ToFirstPost(this CreateDiscussionDto dto, string authorId)
        => new()
        {
            Body = dto.Body,
            AuthorId = authorId
        };
}
