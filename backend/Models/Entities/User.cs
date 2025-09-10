using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Avatar { get; set; }
    
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}