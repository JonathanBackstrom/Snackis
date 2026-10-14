using Snackis.Domain.Enums;

namespace Snackis.Application.DTOs;

public record PostDto(
    int Id,
    string Body,
    string AuthorId,
    string AuthorName,
    string? AvatarPath,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsDeleted,
    IReadOnlyList<PostDto> Replies,
    IReadOnlyDictionary<ReactionType, int>? ReactionCounts = null,
    ReactionType? MyReaction = null,
    IReadOnlyList<string>? ImagePaths = null
);
