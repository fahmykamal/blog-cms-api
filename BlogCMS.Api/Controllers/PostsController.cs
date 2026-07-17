using System.Security.Claims;
using BlogCMS.Api.DTOs;
using BlogCMS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? published = true)
    {
        var posts = await _postService.GetAllAsync(published);
        return Ok(posts);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _postService.GetBySlugAsync(slug);
        if (post is null) return NotFound(new { error = $"Post with slug '{slug}' not found." });
        return Ok(post);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(PostRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = await _postService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetBySlug), new { slug = post.Slug }, post);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, PostRequest request)
    {
        var post = await _postService.UpdateAsync(id, request);
        if (post is null) return NotFound(new { error = $"Post with ID {id} not found." });
        return Ok(post);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _postService.DeleteAsync(id);
        if (!deleted) return NotFound(new { error = $"Post with ID {id} not found." });
        return NoContent();
    }
}
