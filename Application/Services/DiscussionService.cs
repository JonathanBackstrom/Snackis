using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class DiscussionService(IDiscussionRepository discussionRepository,
    IPostRepository postRepository, IBannedWordService bannedWordService,
    IProfanityFilter profanityFilter, IReactionRepository reactionRepository, IPostImageRepository postImageRepository) : IDiscussionService
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

    public async Task<DiscussionDetailDto> GetDetailAsync(int id, string? userId)
    {
        var discussion = await discussionRepository.GetIdAsync(id)
            ?? throw new NotFoundException($"Diskussion med id {id} hittades inte");

        var posts = await postRepository.GetByDiscussionIdAsync(id);
        var bannedWords = await bannedWordService.GetWordsAsync();
        var reactions = await reactionRepository.GetByDiscussionIdAsync(id);
        var images = await postImageRepository.GetByDiscussionIdAsync(id);

        var context = new TreeContext(posts, bannedWords, reactions.ToLookup(r => r.PostId), images.ToLookup(i => i.PostId), userId);

        var dto = discussion.ToDetailDto(BuildTree(context, null));

        return dto with { Title = profanityFilter.Censor(dto.Title, bannedWords) };
    }

    private IReadOnlyList<PostDto> BuildTree(TreeContext context, int? parentPostId)
    => context.All
        .Where(p => p.ParentPostId == parentPostId)
        .Select(p => p.ToDto(BuildTree(context, p.Id)) with
        {
            Body = profanityFilter.Censor(p.Body, context.BannedWords),
            ReactionCounts = context.Reactions[p.Id]
                .GroupBy(r => r.Type)
                .ToDictionary(g => g.Key, g => g.Count()),

            MyReaction = context.Reactions[p.Id]
                .FirstOrDefault(r => r.UserId == context.UserId)?.Type,

            ImagePaths = context.Images[p.Id]
                .Select(i => i.Path)
                .ToList()

        })
        .ToList();

    public async Task SetLockedAsync(int discussionId, bool isLocked)
    {
        var discussion = await discussionRepository.GetIdAsync(discussionId)
            ?? throw new NotFoundException($"Diskussion med id {discussionId} hittades inte");

        discussion.IsLocked = isLocked;
        await discussionRepository.UpdateAsync(discussion);

    }

    private sealed record TreeContext(
        IReadOnlyList<Post> All,
        IReadOnlyList<string> BannedWords,
        ILookup<int, Reaction> Reactions,
        ILookup<int, PostImage> Images,
        string? UserId);
}
