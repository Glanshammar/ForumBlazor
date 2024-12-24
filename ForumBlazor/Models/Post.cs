namespace ForumBlazor.Models;

using System.ComponentModel.DataAnnotations;

public class Post
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters.")]
    public string Content { get; set; }

    public string AuthorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ThemeId { get; set; }
    public Theme Theme { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();
}