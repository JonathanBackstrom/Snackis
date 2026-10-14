using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Tests;

internal class FakePostImageRepository : IPostImageRepository
{
    private readonly List<PostImage> images = [];
    private int nextId = 1;

    public FakePostImageRepository(params PostImage[] seed)
    {
        foreach (var image in seed)
        {
            Lagg(image);
        }
    }

    public IReadOnlyList<PostImage> Bilder => images;

    public Task AddAsync(PostImage image)
    {
        Lagg(image);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<PostImage>> GetByDiscussionIdAsync(int discussionId)
        => Task.FromResult<IReadOnlyList<PostImage>>(images.ToList());

    private void Lagg(PostImage image)
    {
        if (image.Id == 0)
        {
            image.Id = nextId++;
        }
        else if (image.Id >= nextId)
        {
            nextId = image.Id + 1;
        }

        images.Add(image);
    }
}
