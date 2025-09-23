namespace learnyx.Models.Responses;

public class CourseSearchResult
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int Students { get; set; }
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public string Duration { get; set; } = string.Empty;
    public int Lessons { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string? ThumbnailUrl { get; set; }
    public bool IsBestseller { get; set; }
    public DateTime UpdatedAt { get; set; }
}