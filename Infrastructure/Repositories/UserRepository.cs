using Microsoft.EntityFrameworkCore;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;
using Snackis.Infrastructure.Data;

namespace Snackis.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<ApplicationUser?> GetByIdAsync(string id) => context.Users.FirstOrDefaultAsync(u => u.Id == id);


    public async Task UpdateAsync(ApplicationUser user)
    {
        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
