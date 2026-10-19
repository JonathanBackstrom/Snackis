using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class CreatePostDto
{
    [Required(ErrorMessage = "Skriv något i inlägget")]
    public string Body { get; set; } = string.Empty;
    public int DiscussionId { get; set; }
    public int? ParentPostId { get; set; }
}
