namespace Snackis.Application.DTOs;

public record ProfileDto(
    string Id,
    string DisplayName,
    string? AvatarPath,
    string? Bio,
    DateTime CreatedAt
    );

