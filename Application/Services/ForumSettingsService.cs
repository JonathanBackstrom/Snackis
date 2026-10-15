using Snackis.Application.DTOs;
using Snackis.Application.Interfaces;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class ForumSettingsService(IForumSettingsRepository forumSettingsRepository) : IForumSettingsService
{
    public async Task<string> GetTitleAsync()
    {
        var settings = await forumSettingsRepository.GetAsync();
        return settings.Title;
    }

    public async Task UpdateTitleAsync(UpdateForumSettingsDto dto)
    {
        var settings = await forumSettingsRepository.GetAsync();

        settings.Title = dto.Title.Trim();

        await forumSettingsRepository.UpdateAsync(settings);
    }
}
