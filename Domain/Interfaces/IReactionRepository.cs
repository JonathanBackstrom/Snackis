using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IReactionRepository
{
    Task<Reaction?> GetByPostAndUserAsync(int postId, string userId);
    Task<IReadOnlyList<Reaction>> GetByDiscussionIdAsync(int discussionId);
    Task AddAsync(Reaction reaction);
    Task UpdateAsync(Reaction reaction);
    Task DeleteAsync(int id);
}
