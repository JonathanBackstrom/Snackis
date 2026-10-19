namespace Snackis.Application.DTOs;

public record ConversationDetailDto(
    int Id,
    string Title,
    bool IsGroup,
    string OwnerId,
    IReadOnlyList<ParticipantDto> Participants,
    IReadOnlyList<MessageDto> Messages);
