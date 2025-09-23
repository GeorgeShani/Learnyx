namespace learnyx.Models.Requests;

public class SearchCoursesRequest
{
    public string? SearchQuery { get; set; }
    public List<string> Categories { get; set; } = new();
    public string? Level { get; set; }
    public string? PriceRange { get; set; }
    public string SortBy { get; set; } = "popular";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
