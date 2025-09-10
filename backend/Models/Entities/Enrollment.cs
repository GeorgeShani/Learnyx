using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Enrollment : BaseEntity
{
    public DateTime EnrolledAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.ACTIVE;
    public decimal ProgressPercentage { get; set; }
    public string? Notes { get; set; }
    
    public int StudentId { get; set; }
    public virtual User Student { get; set; } = null!;
    
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
}