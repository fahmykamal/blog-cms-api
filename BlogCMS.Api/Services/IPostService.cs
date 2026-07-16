using BlogCMS.Api.DTOs;

namespace BlogCMS.Api.Services;

public interface IPostService
{
    Task<List<PostSummaryResponse>> GetAllAsync(bool? publishedOnly = true);
    Task<PostResponse?> GetBySlugAsync(string slug);
    Task<PostResponse> CreateAsync(PostRequest request, int authorId);
    Task<PostResponse?> UpdateAsync(int id, PostRequest request);
    Task<bool> DeleteAsync(int id);
}
