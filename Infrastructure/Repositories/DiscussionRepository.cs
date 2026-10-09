using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class DiscussionRepository(IDbContextFactory<ApplicationDbContext> factory) : IDiscussionRepository
{
    public async Task<Discussion> AddWithFirstPostAsync(Discussion discussion, Post firstPost)
    {
        await using var context = await factory.CreateDbContextAsync();

        firstPost.Discussion = discussion;

        context.Discussions.Add(discussion);
        context.Posts.Add(firstPost);

        await context.SaveChangesAsync();

        return discussion;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        var discussion = await context.Discussions.FindAsync(id);
        if (discussion is null) return;

        context.Discussions.Remove(discussion);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Discussion>> GetByCategoryIdAsync(int categoryId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Discussions
            .Include(d => d.Author)
            .Where(d => d.CategoryId == categoryId)
            .OrderByDescending(d => d.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Discussion?> GetIdAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Discussions
            .Include(d => d.Author)
            .Include(d => d.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task UpdateAsync(Discussion discussion)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(discussion).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
