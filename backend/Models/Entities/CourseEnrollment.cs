namespace learnyx.Models.Entities;

public class CourseEnrollment : BaseEntity
{
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    public DateTime? CompletionDate { get; set; }
    public decimal AmountPaid { get; set; }
    public bool IsCompleted { get; set; } = false;
    public double Progress { get; set; } = 0; // Percentage 0-100
    
    // Foreign Keys
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
    
    // Navigation Properties
    public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
}