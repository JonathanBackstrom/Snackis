using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class CreateReportDto
{
    public int PostId { get; set; }

    [Required(ErrorMessage = "Beskriv varför du anmäler inlägget")]
    [StringLength(500, ErrorMessage = "Beskrivningen får vara högst 500 tecken")]
    public string Reason { get; set; } = string.Empty;
}
