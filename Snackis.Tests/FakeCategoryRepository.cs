using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakeCategoryRepository : ICategoryRepository
{
    private readonly List<Category> categories = [];
    private int nextId = 1;

    public bool HarDiskussioner { get; set; }

    public FakeCategoryRepository(params Category[] seed)
    {
        foreach (var category in seed)
        {
            category.Id = nextId++;
            categories.Add(category);
        }
    }

    public Task<Category?> GetIdAsync(int id)
        => Task.FromResult(categories.FirstOrDefault(c => c.Id == id));

    public Task<IReadOnlyList<Category>> GetAllAsync()
        => Task.FromResult<IReadOnlyList<Category>>(categories);

    public Task<Category> AddAsync(Category category)
    {
        category.Id = nextId++;
        categories.Add(category);
        return Task.FromResult(category);
    }

    public Task UpdateAsync(Category category) => Task.CompletedTask;

    public Task DeleteAsync(int id)
    {
        var category = categories.FirstOrDefault(c => c.Id == id);
        if (category is not null)
        {
            categories.Remove(category);
        }

        return Task.CompletedTask;
    }

    public Task<bool> HasDiscussionsAsync(int categoryId) => Task.FromResult(HarDiskussioner);
}