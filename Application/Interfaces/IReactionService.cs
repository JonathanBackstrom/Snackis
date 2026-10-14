using Snackis.Domain.Enums;

namespace Snackis.Application.Interfaces;

public interface IReactionService
{
    Task ReactAsync(int postId, string userId, ReactionType type);
}
