using Snackis.Application.DTOs;
using Snackis.Domain.Entities;

namespace Snackis.Application.Mappings;

public static class BannedWordMappings
{
    public static BannedWordDto ToDto(this BannedWord word) => new(word.Id, word.Word);

    public static BannedWord ToEntity(this CreateBannedWordDto dto) => new() { Word = dto.Word.Trim() };
}
