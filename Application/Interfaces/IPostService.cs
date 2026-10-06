using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface IPostService
{
    Task<int> CreateAsync(CreatePostDto dto, string authorId);
    Task SoftDeleteAsync(int postId, string userId, bool isAdmin);
    Task UpdateAsync(UpdatePostDto dto, string userId);
    Task<IReadOnlyList<PostListDto>> GetByDiscussionAsync(int discussionId);

}
