using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationInMonths { get; set; }
    public PlanStatus Status { get; set; } = PlanStatus.ACTIVE;
    public int MaxCourseAccess { get; set; } = -1; // -1 for unlimited
    public bool IsPopular { get; set; } = false;
    public string? Color { get; set; }
    public string[]? Features { get; set; }
    
    public virtual ICollection<UserPlan> UserPlans { get; set; } = new List<UserPlan>();
    public virtual ICollection<PlanCourseAccess> CourseAccess { get; set; } = new List<PlanCourseAccess>();
}