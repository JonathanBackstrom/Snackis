using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Entities;
using Snackis.Domain.Enums;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class ReportService(IReportRepository reportRepository, IPostRepository postRepository, IPostService postService) : IReportService
{
    public async Task CreateAsync(CreateReportDto dto, string reporterId)
    {
        var post = await postRepository.GetIdAsync(dto.PostId)
            ?? throw new NotFoundException($"Inlägget med id {dto.PostId} hittades inte.");

        if (post.AuthorId == reporterId)
            throw new BusinessRuleException("Du kan inte anmäla ditt egna inlägg.");

        if (await reportRepository.HasPendingAsync(dto.PostId, reporterId))
            throw new BusinessRuleException("Du har redan anmält det här inlägget.");

        await reportRepository.AddAsync(dto.ToEntity(reporterId));
    }

    public async Task<IReadOnlyList<ReportListDto>> GetPendingAsync()
    {
        var reports = await reportRepository.GetPendingAsync();

        return reports
            .Select(r => r.ToListDto())
            .ToList();
    }

    public async Task RemovePostAsync(int reportId, string adminId)
    {
        var report = await GetPendingReportAsync(reportId);

        await postService.SoftDeleteAsync(report.PostId, adminId, isAdmin: true);

        await MarkHandledAsync(report, adminId, ReportStatus.Resolved);
    }
    public async Task RejectAsync(int reportId, string adminId)
    {
        var report = await GetPendingReportAsync(reportId);

        await MarkHandledAsync(report, adminId, ReportStatus.Rejected);
    }

    private async Task MarkHandledAsync(Report report, string adminId, ReportStatus status)
    {
        report.Status = status;
        report.HandledById = adminId;
        report.HandledAt = DateTime.UtcNow;

        await reportRepository.UpdateAsync(report);
    }

    private async Task<Report> GetPendingReportAsync(int reportId)
    {
        var report = await reportRepository.GetByIdAsync(reportId)
            ?? throw new NotFoundException($"Anmälan med id {reportId} hittades inte.");

        if (report.Status != ReportStatus.Pending)
            throw new BusinessRuleException("Anmälan är redan handlagd.");

        return report;
    }

}
