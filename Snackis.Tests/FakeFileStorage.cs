using Snackis.Application.Interfaces;

namespace Snackis.Tests;

internal class FakeFileStorage : IFileStorage
{
    private readonly List<string> sparade = [];
    private readonly List<string> raderade = [];

    public IReadOnlyList<string> Sparade => sparade;
    public IReadOnlyList<string> Raderade => raderade;

    public Task<string> SaveAsync(Stream content, string fileName)
    {
        var path = $"/uploads/{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        sparade.Add(path);

        return Task.FromResult(path);
    }

    public Task DeleteAsync(string path)
    {
        raderade.Add(path);
        return Task.CompletedTask;
    }
}
