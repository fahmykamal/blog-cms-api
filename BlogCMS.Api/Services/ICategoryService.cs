using BlogCMS.Api.DTOs;

namespace BlogCMS.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> CreateAsync(CategoryRequest request);
    Task<bool> DeleteAsync(int id);
}
