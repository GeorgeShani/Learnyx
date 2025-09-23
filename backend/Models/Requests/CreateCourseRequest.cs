using System.ComponentModel.DataAnnotations;
using learnyx.Models.Enums;

namespace learnyx.Models.Requests;

public class CreateCourseRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public CourseLevel? Level { get; set; }
    public string Language { get; set; } = "English";
    public decimal Price { get; set; }
    public bool IsFree { get; set; } = false;
    public List<string> Tags { get; set; } = new();
    public List<string> Requirements { get; set; } = new();
    public List<string> LearningOutcomes { get; set; } = new();
    public CourseSettingsRequest Settings { get; set; } = new();
    public List<CourseSectionRequest> Sections { get; set; } = new();
}