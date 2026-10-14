using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Application.Validation;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class ProfileService(IUserRepository userRepository, IFileStorage fileStorage) : IProfileService
{
    public async Task<ProfileDto> GetProfileAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Användare med ID {userId} hittades inte");

        return user.ToProfileDto();
    }

    public async Task UpdateAvatarAsync(string userId, Stream content, string fileName, long sizeInBytes)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Användare med ID {userId} hittades inte");

        ImageRules.Validate(fileName, sizeInBytes);

        var newPath = await fileStorage.SaveAsync(content, fileName);

        if (user.AvatarPath is not null)
            await fileStorage.DeleteAsync(user.AvatarPath);

        user.AvatarPath = newPath;
        await userRepository.UpdateAsync(user);
    }

    public async Task UpdateBioAsync(string userId, string? bio)
    {
        if (bio is not null && bio.Length > 1000)
            throw new BusinessRuleException("Presentation får vara högst 1000 tecken");

        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException($"Användare med ID {userId} hittades inte");
        user.Bio = bio;

        await userRepository.UpdateAsync(user);
    }
}
