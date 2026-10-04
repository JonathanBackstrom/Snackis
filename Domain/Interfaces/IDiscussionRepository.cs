using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IDiscussionRepository
{
    Task<Discussion?> GetIdAsync(int id);
    Task<IReadOnlyList<Discussion>> GetByCategoryIdAsync(int categoryId);
    Task<Discussion> AddWithFirstPostAsync(Discussion discussion, Post firstPost);
    Task UpdateAsync(Discussion discussion);
    Task DeleteAsync(int id);
}
