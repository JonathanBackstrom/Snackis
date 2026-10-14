using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;
public interface IPostImageRepository
{
    Task AddAsync(PostImage image);
    Task<IReadOnlyList<PostImage>> GetByDiscussionIdAsync(int discussionId);
}
