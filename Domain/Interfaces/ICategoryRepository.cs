using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetIdAsync(int id);
    Task<IReadOnlyList<Category>> GetAllAsync();
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(int id);
}
