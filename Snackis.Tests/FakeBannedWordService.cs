using Snackis.Application.DTOs;
using Snackis.Application.Interfaces;

namespace Snackis.Tests;

internal class FakeBannedWordService(params string[] words) : IBannedWordService
{
    private readonly List<string> words = [.. words];

    public Task<IReadOnlyList<string>> GetWordsAsync()
        => Task.FromResult<IReadOnlyList<string>>(words);

    public Task<IReadOnlyList<BannedWordDto>> GetAllAsync()
        => Task.FromResult<IReadOnlyList<BannedWordDto>>(
            words.Select((w, i) => new BannedWordDto(i + 1, w)).ToList());

    public Task AddAsync(CreateBannedWordDto dto)
    {
        words.Add(dto.Word);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        if (id >= 1 && id <= words.Count)
        {
            words.RemoveAt(id - 1);
        }

        return Task.CompletedTask;
    }
}
