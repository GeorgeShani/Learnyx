using learnyx.Models.Responses;

namespace learnyx.Models.DTOs;

public class SearchCoursesDTO
{
    public List<CourseSearchResult> Courses { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
}