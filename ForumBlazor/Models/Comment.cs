namespace ForumBlazor.Models;

using System.ComponentModel.DataAnnotations;

public class Comment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Comment content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters.")]
    public string Content { get; set; }

    public string AuthorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int PostId { get; set; }
    public Post Post { get; set; }
}