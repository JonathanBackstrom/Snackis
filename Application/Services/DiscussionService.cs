using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class DiscussionService(IDiscussionRepository discussionRepository, IPostRepository postRepository) : IDiscussionService
{
    public async Task<int> CreateAsync(CreateDiscussionDto dto, string authorId)
    {
        var discussion = dto.ToEntity(authorId);
        var post = dto.ToFirstPost(authorId);

        var created = await discussionRepository.AddWithFirstPostAsync(discussion, post);
        return created.Id;
    }

    public async Task<IReadOnlyList<DiscussionListDto>> GetByCategoryAsync(int categoryId)
    {
        var discussion = await discussionRepository.GetByCategoryIdAsync(categoryId);
        return discussion.Select(d => d.ToListDto()).ToList();
    }

    public async Task<DiscussionDetailDto> GetDetailAsync(int id)
    {
        var discussion = await discussionRepository.GetIdAsync(id)
            ?? throw new NotFoundException($"Diskussion med id {id} hittades inte");

        var posts = await postRepository.GetByDiscussionIdAsync(id);

        return discussion.ToDetailDto(BuildTree(posts, null));
    }

    private static IReadOnlyList<PostDto> BuildTree(IReadOnlyList<Post> all, int? parentPostId)
        => all.Where(p => p.ParentPostId == parentPostId)
           .Select(p => p.ToDto(BuildTree(all, p.Id)))
           .ToList();


    public async Task SetLockedAsync(int discussionId, bool isLocked)
    {
        var discussion = await discussionRepository.GetIdAsync(discussionId)
            ?? throw new NotFoundException($"Diskussion med id {discussionId} hittades inte");

        discussion.IsLocked = isLocked;
        await discussionRepository.UpdateAsync(discussion);

    }
}
