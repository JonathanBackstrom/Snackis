using System.ComponentModel.DataAnnotations;

namespace Snackis.Application.DTOs;

public class SendMessageDto
{
    public int ConversationId { get; set; }

    [Required(ErrorMessage = "Skriv ett meddelande")]
    [StringLength(2000, ErrorMessage = "Meddelandet får vara högst 2000 tecken")]
    public string Body { get; set; } = string.Empty;
}
