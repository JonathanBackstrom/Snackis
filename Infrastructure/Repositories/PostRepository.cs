using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class PostRepository(ApplicationDbContext context) : IPostRepository
{
    public async Task<Post> AddAsync(Post post)
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        return post;
    }

    public async Task<IReadOnlyList<Post>> GetByDiscussionIdAsync(int discussionId)
    => await context.Posts
        .AsNoTracking()
        .Include(p => p.Author)
        .Where(p => p.DiscussionId == discussionId)
        .OrderBy(p => p.CreatedAt)
        .ToListAsync();

    public async Task<Post?> GetIdAsync(int id)
    => await context.Posts
        .FirstOrDefaultAsync(p => p.Id == id);

    public async Task UpdateAsync(Post post)
    {
        context.Entry(post).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
