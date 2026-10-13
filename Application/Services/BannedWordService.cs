using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Interfaces;

public class BannedWordService(IBannedWordRepository repository) : IBannedWordService
{
    public async Task<IReadOnlyList<string>> GetWordsAsync()
    {
        var words = await repository.GetAllAsync();

        return words.Select(w => w.Word).ToList();
    }

    public async Task<IReadOnlyList<BannedWordDto>> GetAllAsync()
    {
        var words = await repository.GetAllAsync();

        return words.Select(w => w.ToDto()).ToList();
    }

    public async Task AddAsync(CreateBannedWordDto dto)
    {
        var word = dto.Word.Trim();

        if (string.IsNullOrWhiteSpace(word))
            throw new BusinessRuleException("Ordet får inte vara tomt.");

        var existing = await repository.GetAllAsync();

        if (existing.Any(w => string.Equals(w.Word, word, StringComparison.OrdinalIgnoreCase)))
            throw new BusinessRuleException($"\"{word}\" finns redan i listan.");

        await repository.AddAsync(dto.ToEntity());
    }

    public async Task DeleteAsync(int id)
    {
        await repository.DeleteAsync(id);
    }
}