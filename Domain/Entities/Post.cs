namespace Snackis.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public int DiscussionId { get; set; }
    public int? ParentPostId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }

    public ApplicationUser Author { get; set; } = null!;
    public Discussion Discussion { get; set; } = null!;
    public Post? ParentPost { get; set; }
    public ICollection<Post> Replies { get; set; } = new List<Post>();
    public ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();


}