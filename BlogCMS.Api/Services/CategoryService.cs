using AutoMapper;
using BlogCMS.Api.Data;
using BlogCMS.Api.DTOs;
using BlogCMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> CreateAsync(CategoryRequest request)
    {
        var slug = request.Name.ToLowerInvariant().Replace(" ", "-");

        if (await _db.Categories.AnyAsync(c => c.Slug == slug))
            throw new InvalidOperationException("Category already exists.");

        var category = new Category { Name = request.Name, Slug = slug };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null) return false;

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return true;
    }
}
