using Snackis.Application.DTOs;
using Snackis.Domain.Entities;

namespace Snackis.Application.Mappings;

public static class PostMappings
{
    public static PostDto ToDto(this Post post, IReadOnlyList<PostDto> replies) => new(
        post.Id,
        post.Body,
        post.AuthorId,
        post.Author.DisplayName ?? post.Author.UserName ?? "Okänd",
        post.Author.AvatarPath,
        post.CreatedAt,
        post.EditedAt,
        post.IsDeleted,
        replies);

    public static Post ToEntity(this CreatePostDto dto,string authorId)
        => new()
        {
            Body = dto.Body,
            DiscussionId = dto.DiscussionId,
            ParentPostId = dto.ParentPostId,
            AuthorId = authorId
        };

    public static void ApplyTo(this UpdatePostDto dto, Post post)
    {
        post.Body = dto.Body;
    }
}
