using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IBannedWordRepository
{
    Task<IReadOnlyList<BannedWord>> GetAllAsync();
    Task<BannedWord> AddAsync(BannedWord word);
    Task DeleteAsync(int id);
}
