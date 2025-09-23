using learnyx.Data;
using FluentValidation;
using learnyx.Models.DTOs;
using learnyx.Models.Enums;
using learnyx.Models.Entities;
using learnyx.Models.Requests;
using learnyx.Models.Responses;
using learnyx.Models.ValueObjects;
using learnyx.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Services.Implementation;

public class CourseService : ICourseService
{
    private readonly DataContext _context;
    private readonly IAmazonS3Service _fileService;
    private readonly IValidator<CreateCourseRequest> _validator;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        DataContext context, 
        IAmazonS3Service fileService,
        ILogger<CourseService> logger, 
        IValidator<CreateCourseRequest> validator
    ) {
        _context = context;
        _fileService = fileService;
        _logger = logger;
        _validator = validator;
    }
    
    public async Task<SearchCoursesDTO> SearchCoursesAsync(SearchCoursesRequest request)
    {
        try
        {
            // Start with published courses only
            var query = _context.Courses
                .Include(c => c.Teacher)
                .Where(c => c.Status == CourseStatus.Published);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var searchTerm = request.SearchQuery.ToLower();
                query = query.Where(c =>
                    c.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Teacher.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Teacher.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    c.Tags.Any(tag => tag.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)) ||
                    c.Category.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            // Apply category filter
            if (request.Categories.Count != 0)
            {
                query = query.Where(c => request.Categories.Contains(c.Category));
            }

            // Apply level filter
            if (!string.IsNullOrWhiteSpace(request.Level))
            {
                if (Enum.TryParse<CourseLevel>(request.Level, out var levelEnum))
                {
                    query = query.Where(c => c.Level == levelEnum);
                }
            }

            // Apply price filter
            if (!string.IsNullOrWhiteSpace(request.PriceRange))
            {
                query = request.PriceRange switch
                {
                    "free" => query.Where(c => c.Price == 0),
                    "under-50" => query.Where(c => c.Price > 0 && c.Price < 50),
                    "50-100" => query.Where(c => c.Price >= 50 && c.Price <= 100),
                    "over-100" => query.Where(c => c.Price > 100),
                    _ => query
                };
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = request.SortBy switch
            {
                "newest" => query.OrderByDescending(c => c.UpdatedAt),
                "rating" => query.OrderByDescending(c => c.Rating),
                "price-low" => query.OrderBy(c => c.Price),
                "price-high" => query.OrderByDescending(c => c.Price),
                "popular" => query.OrderByDescending(c => c.EnrolledStudents),
                _ => query.OrderByDescending(c => c.EnrolledStudents)
            };

            // Apply pagination
            var courses = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CourseSearchResult
                {
                    Id = c.Id,
                    Title = c.Title,
                    Instructor = $"{c.Teacher.FirstName} {c.Teacher.LastName}",
                    Rating = c.Rating,
                    ReviewCount = c.ReviewCount,
                    Students = c.EnrolledStudents,
                    Price = c.Price,
                    OriginalPrice = c.OriginalPrice,
                    Duration = c.Duration,
                    Lessons = c.TotalLessons,
                    Level = c.Level.ToString(),
                    Category = c.Category,
                    Tags = c.Tags,
                    ThumbnailUrl = c.ThumbnailUrl,
                    IsBestseller = c.IsBestseller,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            return new SearchCoursesDTO
            {
                Courses = courses,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                HasNextPage = request.Page < totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching courses");
            return new SearchCoursesDTO();
        }
    }

    public async Task<List<CategoryDTO>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _context.Courses
                .Where(c => c.Status == CourseStatus.Published)
                .GroupBy(c => c.Category)
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new CategoryDTO
                {
                    Id = g.Key.ToLower().Replace(" ", "-"),
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Count)
                .ToListAsync();

            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return new List<CategoryDTO>();
        }
    }
    
    public async Task<CourseDTO?> CreateCourseAsync(CreateCourseRequest request, int teacherId)
    {
        try
        {
            // Validate instructor
            var teacher = await _context.Users.FindAsync(teacherId);
            if (teacher == null)
            {
                throw new Exception("Teacher not found");
            }
            
            if (teacher.Role != UserRole.Teacher || teacher.Role != UserRole.Admin)
            {
                throw new Exception("Only teachers can create courses");
            }

            // Validate request
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var course = MapRequestToCourse(request, teacherId);
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Create sections and lessons
            var totalLessons = await CreateCourseSectionsAsync(course.Id, request.Sections);

            // Update course with total lessons count
            course.TotalLessons = totalLessons;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Course created successfully: {CourseId} by instructor {InstructorId}", 
                course.Id, teacherId);

            var response = new CourseDTO
            {
                Id = course.Id,
                Title = course.Title,
                Status = course.Status,
                Message = "Course created successfully as draft",
                TotalSections = request.Sections.Count,
                TotalLessons = totalLessons
            };

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating course for teacher {InstructorId}", teacherId);
            return null;
        }
    }

    public async Task<string> UploadCourseMediaAsync(int courseId, int userId, CourseMediaUploadRequest request)
    {
        try
        {
            // Verify course ownership
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId && c.TeacherId == userId);
            
            if (course == null)
            {
                throw new Exception("Course not found or you don't have permission to modify it");
            }

            // Handle file uploads
            if (request.Thumbnail != null)
            {
                var thumbnailUrl = await _fileService.UploadImageToS3(request.Thumbnail, request.Thumbnail.FileName);
                course.ThumbnailUrl = thumbnailUrl;
            }

            if (request.PreviewVideo != null)
            {
                var videoUrl = await _fileService.UploadImageToS3(request.PreviewVideo, request.PreviewVideo.FileName);
                course.PreviewVideoUrl = videoUrl;
            }

            await _context.SaveChangesAsync();

            return "Media uploaded successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading media for course {CourseId}", courseId);
            return "An error occurred while uploading media";
        }
    }

    public async Task<string?> PublishCourseAsync(int courseId, int userId)
    {
        try
        {
            var course = await _context.Courses
                .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.TeacherId == userId);

            if (course == null)
            {
                throw new Exception("Course not found or you don't have permission to modify it");
            }

            course.Status = CourseStatus.Published;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Course {CourseId} published by user {UserId}", courseId, userId);

            return "Course published successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing course {CourseId}", courseId);
            return "An error occurred while publishing the course";
        }
    }
    
    private static Course MapRequestToCourse(CreateCourseRequest request, int teacherId)
    {
        return new Course
        {
            Title = request.Title,
            Subtitle = request.Subtitle ?? string.Empty,
            Description = request.Description ?? string.Empty,
            Category = request.Category ?? string.Empty,
            Level = request.Level ?? CourseLevel.Beginner,
            Language = request.Language,
            Price = request.IsFree ? 0 : request.Price,
            OriginalPrice = request.Price,
            IsFree = request.IsFree,
            TeacherId = teacherId,
            Status = CourseStatus.Draft,
            Tags = request.Tags,
            Requirements = request.Requirements,
            LearningOutcomes = request.LearningOutcomes,
            Settings = new CourseSettings
            {
                EnableQA = request.Settings.EnableQA,
                EnableReviews = request.Settings.EnableReviews,
                EnableDiscussions = request.Settings.EnableDiscussions,
                AutoApprove = request.Settings.AutoApprove,
                IssueCertificates = request.Settings.IssueCertificates,
                SendCompletionEmails = request.Settings.SendCompletionEmails
            }
        };
    }

    private async Task<int> CreateCourseSectionsAsync(int courseId, List<CourseSectionRequest> sectionRequests)
    {
        var totalLessons = 0;

        foreach (var sectionRequest in sectionRequests)
        {
            var section = new CourseSection
            {
                Title = sectionRequest.Title,
                Order = sectionRequest.Order,
                CourseId = courseId
            };

            _context.CourseSections.Add(section);
            await _context.SaveChangesAsync();

            // Create lessons for this section
            foreach (
                var lesson in sectionRequest.Lessons.Select(lessonRequest => new Lesson
                     {
                         Title = lessonRequest.Title,
                         Type = lessonRequest.Type,
                         Duration = lessonRequest.Duration ?? "0:00",
                         Content = lessonRequest.Content ?? string.Empty,
                         IsFree = lessonRequest.IsFree,
                         Order = lessonRequest.Order,
                         Resources = lessonRequest.Resources,
                         SectionId = section.Id
                     }
                )
            ) {
                _context.Lessons.Add(lesson);
                totalLessons++;
            }
        }

        await _context.SaveChangesAsync();
        return totalLessons;
    }
}