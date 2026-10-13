using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class DiscussionService(IDiscussionRepository discussionRepository, IPostRepository postRepository, IBannedWordService bannedWordService, IProfanityFilter profanityFilter) : IDiscussionService
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
        var bannedWords = await bannedWordService.GetWordsAsync();

        return discussion
            .Select(d => d.ToListDto() with { Title = profanityFilter.Censor(d.Title, bannedWords) })
            .ToList();
    }

    public async Task<DiscussionDetailDto> GetDetailAsync(int id)
    {
        var discussion = await discussionRepository.GetIdAsync(id)
            ?? throw new NotFoundException($"Diskussion med id {id} hittades inte");

        var posts = await postRepository.GetByDiscussionIdAsync(id);

        var bannedWords = await bannedWordService.GetWordsAsync();

        var dto = discussion.ToDetailDto(BuildTree(posts, null, bannedWords));

        return dto with { Title = profanityFilter.Censor(dto.Title, bannedWords) };
    }

    private IReadOnlyList<PostDto> BuildTree(IReadOnlyList<Post> all, int? parentPostId, IReadOnlyList<string> bannedWords)
        => all.Where(p => p.ParentPostId == parentPostId)
           .Select(p => p.ToDto(BuildTree(all, p.Id, bannedWords)) with { Body = profanityFilter.Censor(p.Body, bannedWords) })
           .ToList();

    public async Task SetLockedAsync(int discussionId, bool isLocked)
    {
        var discussion = await discussionRepository.GetIdAsync(discussionId)
            ?? throw new NotFoundException($"Diskussion med id {discussionId} hittades inte");

        discussion.IsLocked = isLocked;
        await discussionRepository.UpdateAsync(discussion);

    }
}
