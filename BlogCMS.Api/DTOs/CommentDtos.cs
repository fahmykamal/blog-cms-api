namespace BlogCMS.Api.DTOs;

public class CommentRequest
{
    public string Content { get; set; } = string.Empty;
}

public class CommentResponse
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public AuthorDto Author { get; set; } = null!;
    public int PostId { get; set; }
}
