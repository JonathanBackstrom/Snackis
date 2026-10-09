using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class CategoryRepository(IDbContextFactory<ApplicationDbContext> factory) : ICategoryRepository
{
    public async Task<Category> AddAsync(Category category)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        var category = await context.Categories.FindAsync(id);
        if (category is null) return;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Categories
           .AsNoTracking()
           .OrderBy(c => c.SortOrder)
           .ThenBy(c => c.Name)
           .ToListAsync();
    }

    public async Task<Category?> GetIdAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Categories.FindAsync(id);
    }

    public async Task UpdateAsync(Category category)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(category).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task<bool> HasDiscussionsAsync(int categoryId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Discussions.AnyAsync(d => d.CategoryId == categoryId);
    }
}
