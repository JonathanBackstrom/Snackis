using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class PostService(IPostRepository postRepository, IDiscussionRepository discussionRepository) : IPostService
{
    public async Task<int> CreateAsync(CreatePostDto dto, string authorId)
    {
        var discussion = await discussionRepository.GetIdAsync(dto.DiscussionId)
            ?? throw new NotFoundException($"Diskussion med id {dto.DiscussionId} hittades inte.");

        if (discussion.IsLocked)
            throw new BusinessRuleException("Tråden är låst");
        
        await ValidateParentAsync(dto.ParentPostId, dto.DiscussionId);

        var post = await postRepository.AddAsync(dto.ToEntity(authorId));
        return post.Id;
    }
    private async Task ValidateParentAsync(int? parentPostId, int discussionId)
    {
        if (parentPostId is null) return;

        var parent = await postRepository.GetIdAsync(parentPostId.Value)
            ?? throw new NotFoundException($"Inlägg med id {parentPostId} hittades inte.");

        if (parent.DiscussionId != discussionId)
            throw new BusinessRuleException("Inlägget tillhör en annan diskussion.");
    }
    public async Task SoftDeleteAsync(int postId, string userId, bool isAdmin)
    {
        var post = await postRepository.GetIdAsync(postId)
            ?? throw new NotFoundException($"Inlägg med id {postId} hittades inte.");

        if (post.AuthorId != userId && !isAdmin)
            throw new BusinessRuleException("Du har inte behörighet att radera detta inlägg.");

        post.IsDeleted = true;
        await postRepository.UpdateAsync(post);
    }
    public async Task UpdateAsync(UpdatePostDto dto, string userId)
    {
        var post = await postRepository.GetIdAsync(dto.Id)
            ?? throw new NotFoundException($"Inlägg med id {dto.Id} hittades inte.");

        if (post.AuthorId != userId)
            throw new BusinessRuleException("Du har inte behörighet att redigera detta inlägg.");

        dto.ApplyTo(post);
        post.EditedAt = DateTime.UtcNow;
        await postRepository.UpdateAsync(post);

    }
}
