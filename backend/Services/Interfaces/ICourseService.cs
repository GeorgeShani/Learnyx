using learnyx.Models.DTOs;
using learnyx.Models.Requests;

namespace learnyx.Services.Interfaces;

public interface ICourseService
{
    Task<SearchCoursesDTO> SearchCoursesAsync(SearchCoursesRequest request);
    Task<List<CategoryDTO>> GetCategoriesAsync();
    Task<CourseDTO?> CreateCourseAsync(CreateCourseRequest request, int teacherId);
    Task<string> UploadCourseMediaAsync(int courseId, int userId, CourseMediaUploadRequest request);
    Task<string?> PublishCourseAsync(int courseId, int userId);
}