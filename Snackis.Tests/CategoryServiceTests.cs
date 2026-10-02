using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Services;
using Snackis.Domain.Entities;

namespace Snackis.Tests;

public class CategoryServiceTests
{
    [Fact]
    public async Task CreateAsync_UtanForalder_SkaparToppnivakategori()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryService(repository);

        var skapad = await service.CreateAsync(new CreateCategoryDto { Name = "Fordon" });

        Assert.Equal("Fordon", skapad.Name);
        Assert.Single(await service.GetTopLevelAsync());
    }

    [Fact]
    public async Task CreateAsync_MedToppnivakategoriSomForalder_Lyckas()
    {
        var fordon = new Category { Name = "Fordon" };
        var repository = new FakeCategoryRepository(fordon);
        var service = new CategoryService(repository);

        var skapad = await service.CreateAsync(new CreateCategoryDto
        {
            Name = "Bilsnack",
            ParentCategoryId = fordon.Id
        });

        Assert.Equal("Bilsnack", skapad.Name);
    }

    [Fact]
    public async Task CreateAsync_MedUnderkategoriSomForalder_KastarBusinessRuleException()
    {
        var fordon = new Category { Name = "Fordon" };
        var bilsnack = new Category { Name = "Bilsnack" };
        var repository = new FakeCategoryRepository(fordon, bilsnack);
        bilsnack.ParentCategoryId = fordon.Id;

        var service = new CategoryService(repository);

        var dto = new CreateCategoryDto { Name = "Volvo", ParentCategoryId = bilsnack.Id };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_MedOkandForalder_KastarNotFoundException()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryService(repository);

        var dto = new CreateCategoryDto { Name = "Bilsnack", ParentCategoryId = 99 };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ForstaKategorin_FarSortOrderEtt()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryService(repository);

        var skapad = await service.CreateAsync(new CreateCategoryDto { Name = "Fordon" });

        Assert.Equal(1, skapad.SortOrder);
    }

    [Fact]
    public async Task CreateAsync_SatterNastaSortOrderBlandSyskon()
    {
        var fordon = new Category { Name = "Fordon", SortOrder = 1 };
        var mat = new Category { Name = "Mat", SortOrder = 2 };
        var repository = new FakeCategoryRepository(fordon, mat);
        var service = new CategoryService(repository);

        var skapad = await service.CreateAsync(new CreateCategoryDto { Name = "Datorer" });

        Assert.Equal(3, skapad.SortOrder);
    }

    [Fact]
    public async Task CreateAsync_RaknarSortOrderPerForalder()
    {
        var fordon = new Category { Name = "Fordon", SortOrder = 1 };
        var mat = new Category { Name = "Mat", SortOrder = 2 };
        var repository = new FakeCategoryRepository(fordon, mat);
        var service = new CategoryService(repository);

        var skapad = await service.CreateAsync(new CreateCategoryDto
        {
            Name = "Bilsnack",
            ParentCategoryId = fordon.Id
        });

        Assert.Equal(1, skapad.SortOrder);
    }

    [Fact]
    public async Task DeleteAsync_MedUnderkategorier_KastarBusinessRuleException()
    {
        var fordon = new Category { Name = "Fordon" };
        var bilsnack = new Category { Name = "Bilsnack" };
        var repository = new FakeCategoryRepository(fordon, bilsnack);
        bilsnack.ParentCategoryId = fordon.Id;

        var service = new CategoryService(repository);

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.DeleteAsync(fordon.Id));
    }

    [Fact]
    public async Task DeleteAsync_MedDiskussioner_KastarBusinessRuleException()
    {
        var fordon = new Category { Name = "Fordon" };
        var repository = new FakeCategoryRepository(fordon) { HarDiskussioner = true };
        var service = new CategoryService(repository);

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.DeleteAsync(fordon.Id));
    }

    [Fact]
    public async Task DeleteAsync_UtanHinder_RaderarKategorin()
    {
        var fordon = new Category { Name = "Fordon" };
        var repository = new FakeCategoryRepository(fordon);
        var service = new CategoryService(repository);

        await service.DeleteAsync(fordon.Id);

        Assert.Empty(await service.GetTreeAsync());
    }

    [Fact]
    public async Task DeleteAsync_OkantId_KastarNotFoundException()
    {
        var repository = new FakeCategoryRepository();
        var service = new CategoryService(repository);

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(99));
    }

    [Fact]
    public async Task UpdateAsync_MedSigSjalvSomForalder_KastarBusinessRuleException()
    {
        var fordon = new Category { Name = "Fordon" };
        var repository = new FakeCategoryRepository(fordon);
        var service = new CategoryService(repository);

        var dto = new UpdateCategoryDto
        {
            Id = fordon.Id,
            Name = "Fordon",
            ParentCategoryId = fordon.Id
        };

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.UpdateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_AndrarNamnet()
    {
        var fordon = new Category { Name = "Fordon" };
        var repository = new FakeCategoryRepository(fordon);
        var service = new CategoryService(repository);

        await service.UpdateAsync(new UpdateCategoryDto { Id = fordon.Id, Name = "Fordonssnack" });

        Assert.Equal("Fordonssnack", fordon.Name);
    }

    [Fact]
    public async Task GetTreeAsync_LaggerUnderkategorierUnderSinForalder()
    {
        var fordon = new Category { Name = "Fordon" };
        var bilsnack = new Category { Name = "Bilsnack" };
        var batsnack = new Category { Name = "Batsnack" };
        var repository = new FakeCategoryRepository(fordon, bilsnack, batsnack);
        bilsnack.ParentCategoryId = fordon.Id;
        batsnack.ParentCategoryId = fordon.Id;

        var service = new CategoryService(repository);

        var trad = await service.GetTreeAsync();

        var rot = Assert.Single(trad);
        Assert.Equal("Fordon", rot.Name);
        Assert.Equal(2, rot.SubCategories.Count);
    }

    [Fact]
    public async Task GetTopLevelAsync_ReturnerarInteUnderkategorier()
    {
        var fordon = new Category { Name = "Fordon" };
        var bilsnack = new Category { Name = "Bilsnack" };
        var repository = new FakeCategoryRepository(fordon, bilsnack);
        bilsnack.ParentCategoryId = fordon.Id;

        var service = new CategoryService(repository);

        var toppniva = await service.GetTopLevelAsync();

        Assert.Single(toppniva);
        Assert.Equal("Fordon", toppniva[0].Name);
    }
}
