using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakePostRepository : IPostRepository
{
    private readonly List<Post> posts = [];
    private int nextId = 1;

    public FakePostRepository(params Post[] seed)
    {
        foreach (var post in seed)
        {
            Lagg(post);
        }
    }

    public Task<Post?> GetIdAsync(int id)
        => Task.FromResult(posts.FirstOrDefault(p => p.Id == id));

    public Task<IReadOnlyList<Post>> GetByDiscussionIdAsync(int discussionId)
        => Task.FromResult<IReadOnlyList<Post>>(posts
            .Where(p => p.DiscussionId == discussionId)
            .OrderBy(p => p.CreatedAt)
            .ToList());

    public Task<Post> AddAsync(Post post)
    {
        Lagg(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post) => Task.CompletedTask;

    private void Lagg(Post post)
    {
        if (post.Id == 0)
        {
            post.Id = nextId++;
        }
        else if (post.Id >= nextId)
        {
            nextId = post.Id + 1;
        }

        post.Author ??= new ApplicationUser { Id = post.AuthorId, DisplayName = "Testanvandare" };

        posts.Add(post);
    }
}
