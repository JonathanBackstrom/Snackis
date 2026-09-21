namespace Snackis.Domain.Entities;

public class Conversation
{
    public int Id { get; set; }
    public bool IsGroup { get; set; }
    public string? Name { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
