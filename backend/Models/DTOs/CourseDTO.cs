using learnyx.Models.Enums;
using learnyx.Models.Entities;

namespace learnyx.Models.DTOs;

public class CourseDTO : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public CourseStatus Status { get; set; }
    public string? Thumbnail { get; set; }
    public DateTime? PublishedAt { get; set; }
}