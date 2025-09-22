namespace learnyx.Models.Entities;

public class CourseCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int CourseCount { get; set; } = 0;
    
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}