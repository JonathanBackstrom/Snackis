using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IForumSettingsService
{
    Task<string> GetTitleAsync();
    Task UpdateTitleAsync(UpdateForumSettingsDto dto);
}
