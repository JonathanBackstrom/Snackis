using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetIdAsync(int id);
    Task<IReadOnlyList<Post>> GetByDiscussionIdAsync(int discussionId);
    Task<Post> AddAsync(Post post);
    Task UpdateAsync(Post post);
}
