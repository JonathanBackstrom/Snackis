using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakeUserRepository : IUserRepository
{
    private readonly List<ApplicationUser> users = [];

    public FakeUserRepository(params ApplicationUser[] seed) => users.AddRange(seed);

    public Task<ApplicationUser?> GetByIdAsync(string id)
        => Task.FromResult(users.FirstOrDefault(u => u.Id == id));

    public Task UpdateAsync(ApplicationUser user) => Task.CompletedTask;

    public Task<IReadOnlyList<ApplicationUser>> SearchByNameAsync(string term)
        => Task.FromResult<IReadOnlyList<ApplicationUser>>(users
            .Where(u => u.DisplayName is not null
                && u.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList());
}
