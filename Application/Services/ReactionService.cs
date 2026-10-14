using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Domain.Entities;
using Snackis.Domain.Enums;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class ReactionService(IReactionRepository reactionRepository, IPostRepository postRepository) : IReactionService
{
    public async Task ReactAsync(int postId, string userId, ReactionType type)
    {
        var post = await postRepository.GetIdAsync(postId)
            ?? throw new NotFoundException($"Inlägg med id {postId} hittades inte");

        if (post.IsDeleted)
            throw new BusinessRuleException("Du kan inte reagera på ett borttaget inlägg");

        var existing = await reactionRepository.GetByPostAndUserAsync(postId, userId);

        if (existing is null)
        {
            await reactionRepository.AddAsync(new Reaction 
            {
                PostId = postId,
                UserId = userId,
                Type = type
            });
            return;
        }
        
        if (existing.Type == type)
        {
            await reactionRepository.DeleteAsync(existing.Id);
            return;
        }

        existing.Type = type;
        await reactionRepository.UpdateAsync(existing);
    }
}
