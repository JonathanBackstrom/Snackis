using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(string userId);
    Task UpdateAvatarAsync(string userId, Stream content, string fileName, long sizeInBytes);
    Task UpdateBioAsync(string userId, string? bio);
}
