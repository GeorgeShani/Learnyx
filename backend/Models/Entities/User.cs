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
    
    public string AuthProvider { get; set; } = "Local"; // "Google", "Facebook", "Local (JWT)"
    public string? GoogleId { get; set; }
    public string? FacebookId { get; set; }
    
    public string? VerificationCode { get; set; }
    public DateTime? CodeDeadline { get; set; }
    
    public string? PasswordResetToken { get; set; }
    public DateTime? TokenDeadline { get; set; }
    
    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
    
    public bool IsTeacher => Role == UserRole.Teacher || Role == UserRole.Admin;
    public int TotalStudents => CreatedCourses.Sum(c => c.EnrolledStudents);
    public double AverageRating => CreatedCourses.Any() ? CreatedCourses.Average(c => c.Rating) : 0;
}