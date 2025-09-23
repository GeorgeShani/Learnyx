using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class CourseDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalSections { get; set; }
    public int TotalLessons { get; set; }
}