using AutoMapper;
using BlogCMS.Api.Data;
using BlogCMS.Api.DTOs;
using BlogCMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Api.Services;

public class PostService : IPostService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public PostService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<PostSummaryResponse>> GetAllAsync(bool? publishedOnly = true)
    {
        var query = _db.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Tags).ThenInclude(pt => pt.Tag)
            .Include(p => p.Comments)
            .AsQueryable();

        if (publishedOnly == true)
            query = query.Where(p => p.IsPublished);

        var posts = await query
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<PostSummaryResponse>>(posts);
    }

    public async Task<PostResponse?> GetBySlugAsync(string slug)
    {
        var post = await _db.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.Tags).ThenInclude(pt => pt.Tag)
            .Include(p => p.Comments).ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        return post is null ? null : _mapper.Map<PostResponse>(post);
    }

    public async Task<PostResponse> CreateAsync(PostRequest request, int authorId)
    {
        var slug = GenerateSlug(request.Title);

        if (await _db.Posts.AnyAsync(p => p.Slug == slug))
            throw new InvalidOperationException("A post with this title already exists.");

        if (!await _db.Categories.AnyAsync(c => c.Id == request.CategoryId))
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        var post = new Post
        {
            Title = request.Title,
            Slug = slug,
            Content = request.Content,
            Excerpt = request.Excerpt,
            FeaturedImageUrl = request.FeaturedImageUrl,
            IsPublished = request.IsPublished,
            PublishedAt = request.IsPublished ? DateTime.UtcNow : null,
            AuthorId = authorId,
            CategoryId = request.CategoryId,
        };

        if (request.TagIds.Count > 0)
        {
            var tags = await _db.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync();
            foreach (var tag in tags)
                post.Tags.Add(new PostTag { Tag = tag });
        }

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return (await GetBySlugAsync(post.Slug))!;
    }

    public async Task<PostResponse?> UpdateAsync(int id, PostRequest request)
    {
        var post = await _db.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null) return null;

        if (!await _db.Categories.AnyAsync(c => c.Id == request.CategoryId))
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        post.Title = request.Title;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.FeaturedImageUrl = request.FeaturedImageUrl;
        post.CategoryId = request.CategoryId;
        post.UpdatedAt = DateTime.UtcNow;

        if (request.IsPublished && !post.IsPublished)
            post.PublishedAt = DateTime.UtcNow;
        post.IsPublished = request.IsPublished;

        _db.PostTags.RemoveRange(post.Tags);
        if (request.TagIds.Count > 0)
        {
            var tags = await _db.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync();
            foreach (var tag in tags)
                post.Tags.Add(new PostTag { Tag = tag });
        }

        await _db.SaveChangesAsync();
        return await GetBySlugAsync(post.Slug);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post is null) return false;

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return true;
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("--", "-");

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        return slug.Trim('-');
    }
}
