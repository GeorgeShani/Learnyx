using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Assignment : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxPoints { get; set; } = 100;
    public SubmissionType SubmissionType { get; set; } = SubmissionType.File;
    public string Instructions { get; set; } = string.Empty;
    public bool AllowLateSubmission { get; set; } = true;
    public int LatePenaltyPercentage { get; set; } = 10; // Per day late
    public bool IsVisible { get; set; } = true;
    public int Order { get; set; }
    
    // Foreign Keys
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public int? LessonId { get; set; } // Optional: Assignment can be tied to specific lesson
    public Lesson? Lesson { get; set; }
    
    // Navigation Properties
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public ICollection<AssignmentResource> Resources { get; set; } = new List<AssignmentResource>();
}