using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class UserRepository(IDbContextFactory<ApplicationDbContext> factory) : IUserRepository
{
    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IReadOnlyList<ApplicationUser>> SearchByNameAsync(string term)
    {
        await using var context = await factory.CreateDbContextAsync();

        return await context.Users
            .AsNoTracking()
            .Where(u => u.DisplayName != null && u.DisplayName.Contains(term))
            .OrderBy(u => u.DisplayName)
            .Take(10)
            .ToListAsync();
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        await using var context = await factory.CreateDbContextAsync();

        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
