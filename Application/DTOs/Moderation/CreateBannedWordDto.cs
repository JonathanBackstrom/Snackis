using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class CreateBannedWordDto
{
    [Required(ErrorMessage = "Skriv ett ord")]
    [StringLength(100, ErrorMessage = "Ordet får vara högst 100 tecken")]
    public string Word { get; set; } = string.Empty;
}