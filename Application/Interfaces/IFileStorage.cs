namespace Snackis.Application.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, string fileName);
    Task DeleteAsync(string path);
}
