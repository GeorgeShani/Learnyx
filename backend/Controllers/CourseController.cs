using FluentValidation;
using learnyx.Models.DTOs;
using learnyx.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace learnyx.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    [HttpGet("search")]
    [AllowAnonymous] // Public endpoint for course search
    public async Task<ActionResult<SearchCoursesDTO>> SearchCourses([FromQuery] SearchCoursesRequest request)
    {
        var result = await _courseService.SearchCoursesAsync(request);
        return Ok(result);
    }

    [HttpGet("categories")]
    [AllowAnonymous] // Public endpoint for categories
    public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
    {
        var categories = await _courseService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDTO>> CreateCourse([FromBody] CreateCourseRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var teacherId))
            {
                return Unauthorized("Invalid user token");
            }

            var response = await _courseService.CreateCourseAsync(request, teacherId);
            if (response == null)
            {
                return BadRequest(new { Message = "Course creation failed" });
            }

            return Ok(response);
        }
        catch (ValidationException e)
        {
            return BadRequest(new { Message = "Validation failed", e.Errors });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal Server Error", Details = e.Message });
        }
    }

    [HttpPost("{courseId:int}/media")]
    public async Task<IActionResult> UploadCourseMedia(int courseId, [FromForm] CourseMediaUploadRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user token");
            }

            var message = await _courseService.UploadCourseMediaAsync(courseId, userId, request);
            if (message == "An error occurred while uploading media")
            {
                return BadRequest(new { Message = "Media upload failed" });
            }

            return Ok(new { Message = message });
        }
        catch (ValidationException e)
        {
            return BadRequest(new { Message = "Validation failed", e.Errors });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal Server Error", Details = e.Message });
        }
    }

    [HttpPost("{courseId}/publish")]
    public async Task<IActionResult> PublishCourse(int courseId)
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user token");
            }

            var message = await _courseService.PublishCourseAsync(courseId, userId);
            if (message == null)
            {
                return BadRequest(new { Message = "Course publishing failed" });
            }

            return Ok(new { Message = message });
        }
        catch (ValidationException e)
        {
            return BadRequest(new { Message = "Validation failed", e.Errors });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal Server Error", Details = e.Message });
        }
    }
}