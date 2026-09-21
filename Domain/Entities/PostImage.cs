namespace Snackis.Domain.Entities;

public class PostImage
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Path { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post Post { get; set; } = null!;
}