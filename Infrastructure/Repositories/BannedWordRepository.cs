using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class BannedWordRepository(IDbContextFactory<ApplicationDbContext> factory) : IBannedWordRepository
{
    public async Task<BannedWord> AddAsync(BannedWord word)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.BannedWords.Add(word);
        await context.SaveChangesAsync();
        return word;
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await factory.CreateDbContextAsync();

        var word = await context.BannedWords.FindAsync(id);
        if (word is null) return;

        context.BannedWords.Remove(word);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<BannedWord>> GetAllAsync()
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.BannedWords
            .AsNoTracking()
            .OrderBy(b => b.Word)
            .ToListAsync();
    }
}
