namespace Snackis.Application.DTOs;

public record ReportListDto(
    int Id,
    int PostId,
    int DiscussionId,
    string PostBody,
    string PostAuthorName,
    string ReporterName,
    string reason,
    DateTime CreatedAt
    );

