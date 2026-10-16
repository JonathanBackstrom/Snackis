using Snackis.Application.DTOs;
using Snackis.Application.Exceptions;
using Snackis.Application.Interfaces;
using Snackis.Application.Mappings;
using Snackis.Domain.Entities;
using Snackis.Domain.Interfaces;

namespace Snackis.Application.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        await ValidateParentAsync(dto.ParentCategoryId);

        var all = await repository.GetAllAsync();
        var siblings = all.Where(c => c.ParentCategoryId == dto.ParentCategoryId).ToList();

        var entity = dto.ToEntity();
        entity.SortOrder = siblings.Count == 0 ? 1 : siblings.Max(c => c.SortOrder) + 1;

        var created = await repository.AddAsync(entity);
        return created.ToDto([]);
    }

    private async Task ValidateParentAsync(int? parentId)
    {
        if (parentId is null) return;

        var parent = await repository.GetIdAsync(parentId.Value)
            ?? throw new NotFoundException($"Förälderkategori med id {parentId} finns inte");

        if (parent.ParentCategoryId is not null)
            throw new BusinessRuleException("En underkategori kan inte ha egna underkategorier");
    }

    public async Task DeleteAsync(int id)
    {
        _ = await repository.GetIdAsync(id)
           ?? throw new NotFoundException($"Kategori med id {id} finns inte.");

        var all = await repository.GetAllAsync();
        if (all.Any(c => c.ParentCategoryId == id))
            throw new BusinessRuleException("Kategorin har underkategorier och kan inte raderas.");

        if (await repository.HasDiscussionsAsync(id))
            throw new BusinessRuleException("Kategorin innehåller diskussioner och kan inte raderas.");

        await repository.DeleteAsync(id);
    }

    public async Task<UpdateCategoryDto> GetForEditAsync(int id)
    {
        var category = await repository.GetIdAsync(id)
            ?? throw new NotFoundException($"Kategori med id {id} finns inte");

        return category.ToUpdateDto();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetTopLevelAsync()
    {
        var all = await repository.GetAllAsync();
        return all
            .Where(c => c.ParentCategoryId is null)
            .Select(c => c.ToDto([]))
            .ToList();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetTreeAsync()
    {
        var all = await repository.GetAllAsync();
        return BuildTree(all, null);
    }

    public async Task UpdateAsync(UpdateCategoryDto dto)
    {
        var category = await repository.GetIdAsync(dto.Id)
            ?? throw new NotFoundException($"Kategori med id {dto.Id} finns inte.");

        if (dto.ParentCategoryId == dto.Id)
            throw new BusinessRuleException("En kategori kan inte vara sin egen förälder.");

        var all = await repository.GetAllAsync();
        var hasChildren = all.Any(c => c.ParentCategoryId == dto.Id);

        if (hasChildren && dto.ParentCategoryId is not null)
            throw new BusinessRuleException("En kategori med underkategorier kan inte flyttas.");

        await ValidateParentAsync(dto.ParentCategoryId);

        dto.ApplyTo(category);
        await repository.UpdateAsync(category);
    }

    private static IReadOnlyList<CategoryDto> BuildTree(IReadOnlyList<Category> all, int? parentId)
    => all.Where(c => c.ParentCategoryId == parentId)
          .Select(c => c.ToDto(BuildTree(all, c.Id)))
          .ToList();

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await repository.GetIdAsync(id)
            ?? throw new NotFoundException($"Kategori med id {id} hittades inte.");

        var subCategories = category.SubCategories
            .Select(s => s.ToDto([]))
            .ToList();

        return category.ToDto(subCategories);
    }
}
