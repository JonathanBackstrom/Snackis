namespace Snackis.Application.DTOs;

public record ConversationDetailDto(
    int Id,
    string Title,
    bool IsGroup,
    IReadOnlyList<string> ParticipantNames,
    IReadOnlyList<MessageDto> Messages);
