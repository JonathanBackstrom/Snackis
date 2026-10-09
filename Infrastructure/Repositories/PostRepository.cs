using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class PostRepository(IDbContextFactory<ApplicationDbContext> factory) : IPostRepository
{
    public async Task<Post> AddAsync(Post post)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Posts.Add(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task<IReadOnlyList<Post>> GetByDiscussionIdAsync(int discussionId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Where(p => p.DiscussionId == discussionId)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Post?> GetIdAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Posts
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Post post)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(post).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
