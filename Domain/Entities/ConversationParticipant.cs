namespace Snackis.Domain.Entities;

public class ConversationParticipant
{
    public int ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastReadAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}