using Snackis.Domain.Enums;

namespace Snackis.Domain.Entities;

public class Report
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string ReporterId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public string? HandledById { get; set; }
    public DateTime? HandledAt { get; set; }

    public Post Post { get; set; } = null!;
    public ApplicationUser Reporter { get; set; } = null!;
    public ApplicationUser? HandledBy { get; set; }

}   