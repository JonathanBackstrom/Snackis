using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class ForumSettingsRepository(IDbContextFactory<ApplicationDbContext> factory) : IForumSettingsRepository
{
    public async Task<ForumSettings> GetAsync()
    {
        await using var context = await factory.CreateDbContextAsync();

        var settings = await context.ForumSettings.FirstOrDefaultAsync();

        if (settings is null)
        {
            settings = new ForumSettings();

            context.ForumSettings.Add(settings);
            await context.SaveChangesAsync();
        }
        return settings;
    }

    public async Task UpdateAsync(ForumSettings settings)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(settings).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
