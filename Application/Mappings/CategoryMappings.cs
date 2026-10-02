using Snackis.Application.DTOs;
using Snackis.Domain.Entities;

namespace Snackis.Application.Mappings;

public static class CategoryMappings
{
    public static CategoryDto ToDto(this Category category, IReadOnlyList<CategoryDto> subCategories) => new
        (category.Id, category.Name, category.Description, category.SortOrder, subCategories);

    public static Category ToEntity(this CreateCategoryDto dto) => new()
    { 
        Name = dto.Name,
        Description = dto.Description,
        ParentCategoryId = dto.ParentCategoryId,
    };

    public static UpdateCategoryDto ToUpdateDto(this Category category)
    => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        ParentCategoryId = category.ParentCategoryId,
    };

    public static void ApplyTo(this UpdateCategoryDto dto, Category category)
    {
        category.Name = dto.Name;
        category.Description = dto.Description;
        category.ParentCategoryId = dto.ParentCategoryId;
    }
}
