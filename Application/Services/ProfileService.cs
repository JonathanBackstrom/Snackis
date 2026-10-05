using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class ProfileService(IUserRepository userRepository, IFileStorage fileStorage) : IProfileService
{
    private static readonly string[] allowedExtensions = [ ".jpg", ".jpeg", ".png", ".webp" ];

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

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new BusinessRuleException("Bild måste vara i JPG, JPEG, PNG eller WEBP-format");
        }

        if (sizeInBytes > 2 * 1024 * 1024)
            throw new BusinessRuleException("Filstorlek överstiger gränsen på 2MB");

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
