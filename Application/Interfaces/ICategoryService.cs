using Snackis.Application.DTOs;

namespace Snackis.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetTreeAsync();
    Task<IReadOnlyList<CategoryDto>> GetTopLevelAsync();
    Task<UpdateCategoryDto> GetForEditAsync(int id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task UpdateAsync(UpdateCategoryDto dto);
    Task DeleteAsync(int id);
}