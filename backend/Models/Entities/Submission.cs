using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Submission : BaseEntity
{
    public string? TextContent { get; set; } // For text submissions
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Pending;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? GradedAt { get; set; }
    public int? Grade { get; set; } // Out of MaxPoints from Assignment
    public string? Feedback { get; set; }
    public bool IsLate { get; set; } = false;
    public int DaysLate { get; set; } = 0;
    
    // Foreign Keys
    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
    
    public int? GradedById { get; set; } // Instructor who graded
    public User? GradedBy { get; set; }
    
    // Navigation Properties
    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();
    
    // Computed Properties
    public double? GradePercentage => Grade.HasValue && Assignment != null ? 
        (double)Grade.Value / Assignment.MaxPoints * 100 : null;
}