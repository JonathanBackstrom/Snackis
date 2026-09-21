using Snackis.Domain.Enums;

namespace Snackis.Domain.Entities;

public class Reaction
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post Post { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}