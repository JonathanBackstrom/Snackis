using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class CreateGroupDto
{
    [Required(ErrorMessage = "Gruppen måste ha ett namn")]
    [StringLength(100, ErrorMessage = "Namnet får vara högst 100 tecken")]
    public string Name { get; set; } = string.Empty;
}
