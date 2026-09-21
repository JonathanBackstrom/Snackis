namespace Snackis.Domain.Entities;

public class Discussion
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsLocked { get; set; }
    public Category Category { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}