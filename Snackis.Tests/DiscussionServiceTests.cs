using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Services;
using Snackis.Domain.Entities;

namespace Snackis.Tests;

public class DiscussionServiceTests
{
    private const string Anna = "anna-id";

    [Fact]
    public async Task CreateAsync_SkaparTradOchForstaInlagget()
    {
        var discussions = new FakeDiscussionRepository();
        var service = new DiscussionService(discussions, new FakePostRepository());

        var dto = new CreateDiscussionDto
        {
            Title = "Vilka dack kor ni?",
            CategoryId = 1,
            Body = "Jag funderar pa Michelin."
        };

        var id = await service.CreateAsync(dto, Anna);

        Assert.True(id > 0);

        var forstaInlagget = Assert.Single(discussions.SkapadeInlagg);
        Assert.Equal("Jag funderar pa Michelin.", forstaInlagget.Body);
        Assert.Equal(Anna, forstaInlagget.AuthorId);
        Assert.Equal(id, forstaInlagget.DiscussionId);
    }

    [Fact]
    public async Task GetDetailAsync_OkantId_KastarNotFoundException()
    {
        var service = new DiscussionService(new FakeDiscussionRepository(), new FakePostRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetDetailAsync(99));
    }

    [Fact]
    public async Task GetDetailAsync_ByggerTradetTvaNivaer()
    {
        var discussions = new FakeDiscussionRepository(
            new Discussion { Title = "Vinterdack", AuthorId = Anna, CategoryId = 1 });

        var rot = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Vilka dack kor ni?" };
        var svar = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin", ParentPostId = 1 };
        var svarPaSvar = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Samma har", ParentPostId = 2 };

        var posts = new FakePostRepository(rot, svar, svarPaSvar);
        var service = new DiscussionService(discussions, posts);

        var detalj = await service.GetDetailAsync(1);

        var rotIDto = Assert.Single(detalj.Posts);
        var svarIDto = Assert.Single(rotIDto.Replies);
        Assert.Single(svarIDto.Replies);
        Assert.Equal("Samma har", svarIDto.Replies[0].Body);
    }

    [Fact]
    public async Task GetDetailAsync_TarMedRaderadeInlagg()
    {
        var discussions = new FakeDiscussionRepository(
            new Discussion { Title = "Vinterdack", AuthorId = Anna, CategoryId = 1 });

        var rot = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Vilka dack kor ni?", IsDeleted = true };
        var svar = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin", ParentPostId = 1 };

        var posts = new FakePostRepository(rot, svar);
        var service = new DiscussionService(discussions, posts);

        var detalj = await service.GetDetailAsync(1);

        var rotIDto = Assert.Single(detalj.Posts);
        Assert.True(rotIDto.IsDeleted);
        Assert.Single(rotIDto.Replies);
    }

    [Fact]
    public async Task GetByCategoryAsync_ReturnerarBaraTradarIKategorin()
    {
        var discussions = new FakeDiscussionRepository(
            new Discussion { Title = "Vinterdack", AuthorId = Anna, CategoryId = 1 },
            new Discussion { Title = "Batmotorer", AuthorId = Anna, CategoryId = 2 });

        var service = new DiscussionService(discussions, new FakePostRepository());

        var trad = await service.GetByCategoryAsync(1);

        var enda = Assert.Single(trad);
        Assert.Equal("Vinterdack", enda.Title);
    }

    [Fact]
    public async Task SetLockedAsync_LaserTraden()
    {
        var discussion = new Discussion { Title = "Vinterdack", AuthorId = Anna, CategoryId = 1 };
        var discussions = new FakeDiscussionRepository(discussion);
        var service = new DiscussionService(discussions, new FakePostRepository());

        await service.SetLockedAsync(discussion.Id, true);

        Assert.True(discussion.IsLocked);
    }

    [Fact]
    public async Task SetLockedAsync_OkantId_KastarNotFoundException()
    {
        var service = new DiscussionService(new FakeDiscussionRepository(), new FakePostRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.SetLockedAsync(99, true));
    }
}
