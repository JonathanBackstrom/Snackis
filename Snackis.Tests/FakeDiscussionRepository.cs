using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakeDiscussionRepository : IDiscussionRepository
{
    private readonly List<Discussion> discussions = [];
    private readonly List<Post> skapadeInlagg = [];
    private int nextId = 1;

    public FakeDiscussionRepository(params Discussion[] seed)
    {
        foreach (var discussion in seed)
        {
            Lagg(discussion);
        }
    }

    public IReadOnlyList<Post> SkapadeInlagg => skapadeInlagg;

    public Task<Discussion?> GetIdAsync(int id)
        => Task.FromResult(discussions.FirstOrDefault(d => d.Id == id));

    public Task<IReadOnlyList<Discussion>> GetByCategoryIdAsync(int categoryId)
        => Task.FromResult<IReadOnlyList<Discussion>>(discussions
            .Where(d => d.CategoryId == categoryId)
            .OrderByDescending(d => d.CreatedAt)
            .ToList());

    public Task<Discussion> AddWithFirstPostAsync(Discussion discussion, Post firstPost)
    {
        Lagg(discussion);

        firstPost.DiscussionId = discussion.Id;
        skapadeInlagg.Add(firstPost);

        return Task.FromResult(discussion);
    }

    public Task UpdateAsync(Discussion discussion) => Task.CompletedTask;

    public Task DeleteAsync(int id)
    {
        var discussion = discussions.FirstOrDefault(d => d.Id == id);
        if (discussion is not null)
        {
            discussions.Remove(discussion);
        }

        return Task.CompletedTask;
    }

    private void Lagg(Discussion discussion)
    {
        if (discussion.Id == 0)
        {
            discussion.Id = nextId++;
        }
        else if (discussion.Id >= nextId)
        {
            nextId = discussion.Id + 1;
        }

        discussion.Author = new ApplicationUser { Id = discussion.AuthorId, DisplayName = "Testanvandare" };
        discussion.Category = new Category { Id = discussion.CategoryId, Name = "Testkategori" };

        discussions.Add(discussion);
    }
}
