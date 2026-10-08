using Snackis.Application.DTOs;
using Snackis.Domain.Entities;

namespace Snackis.Application.Mappings;

public static class ConversationMappings
{
    public static MessageDto ToDto(this Message message)
     => new(
         message.Id,
         message.Body,
         message.SenderId,
         message.Sender.DisplayName ?? message.Sender.UserName ?? "Okänd",
         message.Sender.AvatarPath,
         message.SentAt);
}
