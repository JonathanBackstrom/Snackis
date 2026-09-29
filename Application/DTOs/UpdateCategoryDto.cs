using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class UpdateCategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Namn måste anges")]
    [StringLength(100, ErrorMessage = "Namnet får vara högst 100 tecken")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    public int SortOrder { get; set; }
}
