using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class CreateDiscussionDto
{
    [Required(ErrorMessage = "Rubrik måste anges")]
    [StringLength(200, ErrorMessage = "Rubriken får vara högst 200 tecken")]
    public string Title { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Skriv något i inlägget")]
    public string Body { get; set; } = string.Empty;
}
