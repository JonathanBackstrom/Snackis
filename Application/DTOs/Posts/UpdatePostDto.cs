using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class UpdatePostDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Innehåll måste anges")]
    public string Body { get; set; } = string.Empty;
}
