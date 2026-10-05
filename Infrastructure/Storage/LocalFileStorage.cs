using Snackis.Application.Interfaces;

namespace Snackis.Infrastructure.Storage;

public class LocalFileStorage(string rootPath) : IFileStorage
{
    public async Task<string> SaveAsync(Stream content, string fileName)
    {
        Directory.CreateDirectory(rootPath);

        var extension = Path.GetExtension(fileName);
        var unique = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(rootPath, unique);

        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file);

        return $"/uploads/{unique}";
    }

    public Task DeleteAsync(string path)
    {
        var fullPath = Path.Combine(rootPath, Path.GetFileName(path));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
