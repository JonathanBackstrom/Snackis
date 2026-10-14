using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Services;
using Snackis.Domain.Entities;

namespace Snackis.Tests;

public class PostServiceTests
{
    private const string Anna = "anna-id";
    private const string Bo = "bo-id";

    private static PostService SkapaService(
        FakePostRepository posts,
        FakeDiscussionRepository discussions,
        params string[] bannedWords)
        => new(posts, discussions, new FakeBannedWordService(bannedWords), new ProfanityFilter(), new FakePostImageRepository(), new FakeFileStorage());

    [Fact]
    public async Task CreateAsync_IOppenTrad_SparasOchFarId()
    {
        var discussions = new FakeDiscussionRepository(new Discussion { Title = "Vinterdack", AuthorId = Anna });
        var posts = new FakePostRepository();
        var service = SkapaService(posts, discussions);

        var id = await service.CreateAsync(new CreatePostDto { DiscussionId = 1, Body = "Michelin" }, Bo);

        Assert.True(id > 0);
    }

    [Fact]
    public async Task CreateAsync_ILastTrad_KastarBusinessRuleException()
    {
        var discussions = new FakeDiscussionRepository(
            new Discussion { Title = "Vinterdack", AuthorId = Anna, IsLocked = true });
        var posts = new FakePostRepository();
        var service = SkapaService(posts, discussions);

        var dto = new CreatePostDto { DiscussionId = 1, Body = "Michelin" };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(dto, Bo));
    }

    [Fact]
    public async Task CreateAsync_IOkandTrad_KastarNotFoundException()
    {
        var discussions = new FakeDiscussionRepository();
        var posts = new FakePostRepository();
        var service = SkapaService(posts, discussions);

        var dto = new CreatePostDto { DiscussionId = 99, Body = "Michelin" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(dto, Bo));
    }

    [Fact]
    public async Task CreateAsync_MedOkantForalderinlagg_KastarNotFoundException()
    {
        var discussions = new FakeDiscussionRepository(new Discussion { Title = "Vinterdack", AuthorId = Anna });
        var posts = new FakePostRepository();
        var service = SkapaService(posts, discussions);

        var dto = new CreatePostDto { DiscussionId = 1, ParentPostId = 99, Body = "Michelin" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(dto, Bo));
    }

    [Fact]
    public async Task CreateAsync_MedForalderIAnnanTrad_KastarBusinessRuleException()
    {
        var discussions = new FakeDiscussionRepository(
            new Discussion { Title = "Vinterdack", AuthorId = Anna },
            new Discussion { Title = "Sommardack", AuthorId = Anna });

        var inlaggIAnnanTrad = new Post { DiscussionId = 2, AuthorId = Anna, Body = "Fel trad" };
        var posts = new FakePostRepository(inlaggIAnnanTrad);

        var service = SkapaService(posts, discussions);

        var dto = new CreatePostDto
        {
            DiscussionId = 1,
            ParentPostId = inlaggIAnnanTrad.Id,
            Body = "Michelin"
        };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(dto, Bo));
    }

    [Fact]
    public async Task UpdateAsync_PaEgetInlagg_AndrarTextenOchSatterEditedAt()
    {
        var inlagg = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin" };
        var posts = new FakePostRepository(inlagg);
        var service = SkapaService(posts, new FakeDiscussionRepository());

        await service.UpdateAsync(new UpdatePostDto { Id = inlagg.Id, Body = "Nokian" }, Anna);

        Assert.Equal("Nokian", inlagg.Body);
        Assert.NotNull(inlagg.EditedAt);
    }

    [Fact]
    public async Task UpdateAsync_PaAnnansInlagg_KastarBusinessRuleException()
    {
        var inlagg = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin" };
        var posts = new FakePostRepository(inlagg);
        var service = SkapaService(posts, new FakeDiscussionRepository());

        var dto = new UpdatePostDto { Id = inlagg.Id, Body = "Nokian" };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.UpdateAsync(dto, Bo));
    }

    [Fact]
    public async Task UpdateAsync_OkantInlagg_KastarNotFoundException()
    {
        var posts = new FakePostRepository();
        var service = SkapaService(posts, new FakeDiscussionRepository());

        var dto = new UpdatePostDto { Id = 99, Body = "Nokian" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(dto, Anna));
    }

    [Fact]
    public async Task SoftDeleteAsync_PaEgetInlagg_MarkerasSomRaderat()
    {
        var inlagg = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin" };
        var posts = new FakePostRepository(inlagg);
        var service = SkapaService(posts, new FakeDiscussionRepository());

        await service.SoftDeleteAsync(inlagg.Id, Anna, isAdmin: false);

        Assert.True(inlagg.IsDeleted);
    }

    [Fact]
    public async Task SoftDeleteAsync_SomAdminPaAnnansInlagg_MarkerasSomRaderat()
    {
        var inlagg = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin" };
        var posts = new FakePostRepository(inlagg);
        var service = SkapaService(posts, new FakeDiscussionRepository());

        await service.SoftDeleteAsync(inlagg.Id, Bo, isAdmin: true);

        Assert.True(inlagg.IsDeleted);
    }

    [Fact]
    public async Task SoftDeleteAsync_PaAnnansInlaggUtanAdmin_KastarBusinessRuleException()
    {
        var inlagg = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin" };
        var posts = new FakePostRepository(inlagg);
        var service = SkapaService(posts, new FakeDiscussionRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.SoftDeleteAsync(inlagg.Id, Bo, isAdmin: false));
    }

    [Fact]
    public async Task SoftDeleteAsync_BehallerTexten()
    {
        var inlagg = new Post { DiscussionId = 1, AuthorId = Anna, Body = "Michelin" };
        var posts = new FakePostRepository(inlagg);
        var service = SkapaService(posts, new FakeDiscussionRepository());

        await service.SoftDeleteAsync(inlagg.Id, Anna, isAdmin: false);

        Assert.Equal("Michelin", inlagg.Body);
    }
}
