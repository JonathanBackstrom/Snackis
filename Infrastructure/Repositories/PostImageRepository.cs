using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class PostImageRepository(IDbContextFactory<ApplicationDbContext> factory) : IPostImageRepository
{
    public async Task AddAsync(PostImage image)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.PostImages.Add(image);
        await context.SaveChangesAsync();
        return;
    }

    public async Task<IReadOnlyList<PostImage>> GetByDiscussionIdAsync(int discussionId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.PostImages
            .AsNoTracking()
            .Where(i => i.Post.DiscussionId == discussionId)
            .ToListAsync();
    }
}
