using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class AssignmentDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxPoints { get; set; }
    public SubmissionType SubmissionType { get; set; }
    public bool AllowLateSubmission { get; set; }
    public int LatePenaltyPercentage { get; set; }
    public bool IsVisible { get; set; }
    public int Order { get; set; }
    public int? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public List<AssignmentResourceDTO> Resources { get; set; } = new();
    public SubmissionDTO? StudentSubmission { get; set; } // For student view
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Computed properties
    public bool IsOverdue => DateTime.Now > DueDate;
    public int DaysUntilDue => (int)(DueDate - DateTime.Now).TotalDays;
}