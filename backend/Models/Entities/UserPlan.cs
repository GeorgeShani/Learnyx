using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class UserPlan : BaseEntity
{
    public UserPlanStatus Status { get; set; } = UserPlanStatus.ACTIVE;
    public decimal AmountPaid { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? CancellationReason { get; set; }
    public bool AutoRenew { get; set; } = true;
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? ReminderSentAt { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public int PlanId { get; set; }
    public virtual Plan Plan { get; set; } = null!;
}