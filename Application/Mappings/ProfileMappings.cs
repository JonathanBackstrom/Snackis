using Snackis.Application.DTOs;
using Snackis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snackis.Application.Mappings;

public static class ProfileMappings
{
    public static ProfileDto ToProfileDto(this ApplicationUser user) => new(
        user.Id,
        user.DisplayName ?? user.UserName ?? "Okänd",
        user.AvatarPath,
        user.Bio,
        user.CreatedAt
    );
}
