namespace ForumBlazor.Models;

using System.ComponentModel.DataAnnotations;

public class Theme
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Theme name is required")]
    [StringLength(30, ErrorMessage = "Theme name cannot exceed 30 characters")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Post> Posts { get; set; } = new List<Post>();
}