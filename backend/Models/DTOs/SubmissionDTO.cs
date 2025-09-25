using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class SubmissionDTO
{
    public int Id { get; set; }
    public string? TextContent { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? GradedAt { get; set; }
    public int? Grade { get; set; }
    public string? Feedback { get; set; }
    public bool IsLate { get; set; }
    public int DaysLate { get; set; }
    public List<SubmissionFileDTO> Files { get; set; } = new();
    
    // Student info (for instructor view)
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    
    // Assignment info
    public int AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public int MaxPoints { get; set; }
    
    // Computed properties
    public double? GradePercentage => Grade.HasValue && MaxPoints > 0 ? 
        (double)Grade.Value / MaxPoints * 100 : null;
}