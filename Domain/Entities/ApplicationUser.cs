using Microsoft.AspNetCore.Identity;

namespace Snackis.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? AvatarPath { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Discussion> Discussions { get; set; } = new List<Discussion>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}