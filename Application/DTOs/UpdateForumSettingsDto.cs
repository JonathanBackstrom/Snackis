using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class UpdateForumSettingsDto
{
    [Required(ErrorMessage = "Rubriken måste fyllas i")]
    [StringLength(100, ErrorMessage = "Rubriken får vara högst 100 tecken.")]
    public string Title { get; set; } = string.Empty;
}
