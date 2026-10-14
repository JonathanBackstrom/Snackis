using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task UpdateAsync(ApplicationUser user);
    Task<IReadOnlyList<ApplicationUser>> SearchByNameAsync(string term);

}
