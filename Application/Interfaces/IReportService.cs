using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IReportService
{
    Task CreateAsync(CreateReportDto dto, string reporterId);
    Task<IReadOnlyList<ReportListDto>> GetPendingAsync();
    Task RemovePostAsync(int reportId, string adminId);
    Task RejectAsync(int reportId, string adminId);
}
