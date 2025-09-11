using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.DRAFT;
    public string? Thumbnail { get; set; }
    public DateTime? PublishedAt { get; set; }

    public int TeacherId { get; set; }
    public virtual User Teacher { get; set; } = null!;
    
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public virtual ICollection<PlanCourseAccess> PlanAccess { get; set; } = new List<PlanCourseAccess>();
}