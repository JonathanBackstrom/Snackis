using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Snackis.Application.DTOs;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Namn måste anges")]
    [StringLength(100, ErrorMessage = "Namnet får vara högst 100 tecken")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }
}
