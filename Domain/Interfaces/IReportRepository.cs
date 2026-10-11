using Snackis.Domain.Entities;

namespace Snackis.Domain.Interfaces;

public interface IReportRepository
{
    Task<bool> HasPendingAsync(int postId, string reporterId);
    Task<IReadOnlyList<Report>> GetPendingAsync();
    Task<Report?> GetByIdAsync(int id);
    Task<Report> AddAsync(Report report);
    Task UpdateAsync(Report report);
}
