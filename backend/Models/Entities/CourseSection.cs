namespace learnyx.Models.Entities;

public class CourseSection : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Duration { get; set; } = string.Empty; // e.g., "2 hours"
    
    // Foreign Key
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    // Navigation Properties
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    
    // Computed Properties
    public int LectureCount => Lessons.Count;
}