using System.Security.Claims;
using BlogCMS.Api.DTOs;
using BlogCMS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.Api.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByPost(int postId)
    {
        var comments = await _commentService.GetByPostIdAsync(postId);
        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(int postId, CommentRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var comment = await _commentService.CreateAsync(postId, request, userId);
            return Ok(comment);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Post not found." });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _commentService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
