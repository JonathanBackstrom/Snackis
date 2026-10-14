using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class ReactionRepository(IDbContextFactory<ApplicationDbContext> factory) : IReactionRepository
{
    public async Task AddAsync(Reaction reaction)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Reactions.Add(reaction);
        await context.SaveChangesAsync();
        return;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        var reaction = await context.Reactions.FindAsync(id);
        if (reaction is null) return;

        context.Reactions.Remove(reaction);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Reaction>> GetByDiscussionIdAsync(int discussionId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Reactions
            .AsNoTracking()
            .Where(r => r.Post.DiscussionId == discussionId)
            .ToListAsync();
    }

    public async Task<Reaction?> GetByPostAndUserAsync(int postId, string userId)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Reactions
            .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
    }

    public async Task UpdateAsync(Reaction reaction)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(reaction).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
