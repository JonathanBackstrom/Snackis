using Snackis.Application.Exceptions;

namespace Snackis.Application.Validation;

public static class ImageRules
{
    private const long MaxBytes = 2 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public static void Validate(string fileName, long sizeInBytes)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new BusinessRuleException("Bilden måste vara jpg, png eller webp.");
        if (sizeInBytes > MaxBytes)
            throw new BusinessRuleException("Bilden får vara högst 2 MB.");
    }
}
