using learnyx.Models.Enums;
using learnyx.Models.ValueObjects;

namespace learnyx.Models.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
    public string Language { get; set; } = "English";
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    
    // Teacher (Foreign Key to User)
    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;
    
    // Pricing
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public bool IsFree { get; set; }
    
    // Media
    public string? ThumbnailUrl { get; set; }
    public string? PreviewVideoUrl { get; set; }
    
    // Statistics
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int EnrolledStudents { get; set; }
    public bool IsBestseller { get; set; }
    
    // Course Content
    public int TotalLessons { get; set; }
    public string Duration { get; set; } = string.Empty; // e.g., "40 hours"
    
    // Features
    public bool HasCertificate { get; set; } = true;
    public bool HasDownloadableContent { get; set; } = true;
    public bool HasLifetimeAccess { get; set; } = true;
    
    // SEO and Discovery
    public List<string> Tags { get; set; } = new();
    public List<string> Requirements { get; set; } = new();
    public List<string> LearningOutcomes { get; set; } = new();
    
    // Settings
    public CourseSettings Settings { get; set; } = new();
    
    // Navigation Properties
    public ICollection<CourseSection> Sections { get; set; } = new List<CourseSection>();
    public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
    public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    
    // Computed Properties
    public int GetDiscountPercentage()
    {
        if (OriginalPrice <= 0) return 0;
        return (int)Math.Round((1 - Price / OriginalPrice) * 100);
    }
}