using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsActive { get; set; } = true;
    
    public string AuthProvider { get; set; } = "Local"; // "Google", "Facebook", "Local (JWT)"
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    
    public virtual ICollection<Course> TeacherCourses { get; set; } = new List<Course>();
    
    // Student-specific navigation properties
    public virtual ICollection<UserPlan> UserPlans { get; set; } = new List<UserPlan>();
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    // Common navigation properties
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}