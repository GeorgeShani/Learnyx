namespace learnyx.Models.Entities;

public class Assignment : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxScore { get; set; }
    
    public int CourseId { get; set; }
    public virtual Course Course { get; set; } = null!;
    
    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}