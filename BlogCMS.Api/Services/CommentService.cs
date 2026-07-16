using AutoMapper;
using BlogCMS.Api.Data;
using BlogCMS.Api.DTOs;
using BlogCMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Api.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CommentService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CommentResponse>> GetByPostIdAsync(int postId)
    {
        var comments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<CommentResponse>>(comments);
    }

    public async Task<CommentResponse> CreateAsync(int postId, CommentRequest request, int authorId)
    {
        if (!await _db.Posts.AnyAsync(p => p.Id == postId))
            throw new KeyNotFoundException("Post not found.");

        var comment = new Comment
        {
            Content = request.Content,
            PostId = postId,
            AuthorId = authorId,
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        comment = await _db.Comments
            .Include(c => c.Author)
            .FirstAsync(c => c.Id == comment.Id);

        return _mapper.Map<CommentResponse>(comment);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment is null) return false;

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }
}
