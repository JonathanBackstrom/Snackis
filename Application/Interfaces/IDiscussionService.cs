using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IDiscussionService
{
    Task<IReadOnlyList<DiscussionListDto>> GetByCategoryAsync(int categoryId);
    Task<DiscussionDetailDto> GetDetailAsync(int id, string? userId);
    Task<int> CreateAsync(CreateDiscussionDto dto, string authorId);
    Task SetLockedAsync(int discussionId, bool isLocked);
    Task DeleteAsync(int id);
}
