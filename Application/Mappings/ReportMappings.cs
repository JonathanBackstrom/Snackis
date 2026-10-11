using Snackis.Application.DTOs;
using Snackis.Domain.Entities;

namespace Snackis.Application.Mappings;

public static class ReportMappings
{
    public static ReportListDto ToListDto(this Report report) => new(
        report.Id,
        report.PostId,
        report.Post.DiscussionId,
        report.Post.Body,
        report.Post.Author.DisplayName ?? report.Post.Author.UserName ?? "Okänd",
        report.Reporter.DisplayName ?? report.Reporter.UserName ?? "Okänd",
        report.Reason,
        report.CreatedAt);

    public static Report ToEntity(this CreateReportDto dto, string reporterId) => new()
    {
        PostId = dto.PostId,
        ReporterId = reporterId,
        Reason = dto.Reason
    };
       
}
