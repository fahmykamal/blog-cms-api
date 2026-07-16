namespace BlogCMS.Api.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<PostTag> Posts { get; set; } = new List<PostTag>();
}
