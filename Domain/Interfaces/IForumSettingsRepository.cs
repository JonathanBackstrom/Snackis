using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IForumSettingsRepository
{
    Task<ForumSettings> GetAsync();
    Task UpdateAsync(ForumSettings settings);
}
