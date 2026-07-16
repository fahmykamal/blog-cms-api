using BlogCMS.Api.DTOs;

namespace BlogCMS.Api.Services;

public interface ICommentService
{
    Task<List<CommentResponse>> GetByPostIdAsync(int postId);
    Task<CommentResponse> CreateAsync(int postId, CommentRequest request, int authorId);
    Task<bool> DeleteAsync(int id);
}
