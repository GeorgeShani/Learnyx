namespace learnyx.Models.Entities;

public class CourseReview : BaseEntity
{
    public int Rating { get; set; } // 1-5 stars
    public string Content { get; set; } = string.Empty;
    
    // Foreign Keys
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public int StudentId { get; set; }
    public User Student { get; set; } = null!;
}