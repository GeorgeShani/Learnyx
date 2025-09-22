namespace learnyx.Models.Entities;

public class LessonProgress : BaseEntity
{
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletionDate { get; set; }
    public int WatchTime { get; set; } = 0; // in seconds
    
    // Foreign Keys
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    
    public int EnrollmentId { get; set; }
    public CourseEnrollment Enrollment { get; set; } = null!;
}