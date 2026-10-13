using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IBannedWordService
{
    Task<IReadOnlyList<string>> GetWordsAsync();
    Task<IReadOnlyList<BannedWordDto>> GetAllAsync();
    Task AddAsync(CreateBannedWordDto dto);
    Task DeleteAsync(int id);

}
