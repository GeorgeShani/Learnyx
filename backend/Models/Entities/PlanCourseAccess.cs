namespace learnyx.Models.Entities;

public class PlanCourseAccess : BaseEntity
{

    public DateTime? AccessStartDate { get; set; }
    public DateTime? AccessEndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int PlanId { get; set; }
    public virtual Plan Plan { get; set; } = null!;
    
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
}