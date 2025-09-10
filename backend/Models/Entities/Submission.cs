namespace learnyx.Models.Entities;

public class Submission : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string[]? Attachments { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.Now;
    
    public int StudentId { get; set; }
    public virtual User Student { get; set; } = null!;
    
    public int AssignmentId { get; set; }
    public virtual Assignment Assignment { get; set; } = null!;
}