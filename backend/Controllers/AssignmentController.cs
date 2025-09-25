using learnyx.Models.DTOs;
using learnyx.Models.Requests;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learnyx.Controllers;

[ApiController]
[Route("api/assignments")]
public class AssignmentController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpPost("courses/{courseId:int}")]
    public async Task<ActionResult<AssignmentDTO>> CreateAssignment(int courseId, [FromBody] CreateAssignmentRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out var instructorId))
        {
            return Unauthorized("Invalid user token");
        }

        var (success, assignment, errors) = await _assignmentService.CreateAssignmentAsync(courseId, request, instructorId);

        if (!success)
        {
            return BadRequest(new { Message = "Assignment creation failed", Errors = errors });
        }

        return Ok(assignment);
    }

    [HttpPut("{assignmentId:int}")]
    public async Task<ActionResult<AssignmentDTO>> UpdateAssignment(int assignmentId, [FromBody] CreateAssignmentRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int instructorId))
        {
            return Unauthorized("Invalid user token");
        }

        var (success, assignment, errors) = await _assignmentService.UpdateAssignmentAsync(assignmentId, request, instructorId);

        if (!success)
        {
            return BadRequest(new { Message = "Assignment update failed", Errors = errors });
        }

        return Ok(assignment);
    }

    [HttpDelete("{assignmentId}")]
    public async Task<IActionResult> DeleteAssignment(int assignmentId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int instructorId))
        {
            return Unauthorized("Invalid user token");
        }

        var (success, errors) = await _assignmentService.DeleteAssignmentAsync(assignmentId, instructorId);

        if (!success)
        {
            return BadRequest(new { Message = "Assignment deletion failed", Errors = errors });
        }

        return Ok(new { Message = "Assignment deleted successfully" });
    }

    [HttpGet("{assignmentId}")]
    public async Task<ActionResult<AssignmentDTO>> GetAssignment(int assignmentId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid user token");
        }

        var assignment = await _assignmentService.GetAssignmentAsync(assignmentId, userId);

        if (assignment == null)
        {
            return NotFound("Assignment not found or you don't have access to it");
        }

        return Ok(assignment);
    }

    [HttpGet("courses/{courseId}")]
    public async Task<ActionResult<List<AssignmentDTO>>> GetCourseAssignments(int courseId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid user token");
        }

        var assignments = await _assignmentService.GetCourseAssignmentsAsync(courseId, userId);
        return Ok(assignments);
    }

    [HttpPost("{assignmentId}/submit")]
    public async Task<ActionResult<SubmissionDTO>> SubmitAssignment(int assignmentId, [FromForm] SubmitAssignmentRequest request, [FromForm] List<IFormFile> files)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int studentId))
        {
            return Unauthorized("Invalid user token");
        }

        var (success, submission, errors) = await _assignmentService.SubmitAssignmentAsync(assignmentId, request, files, studentId);

        if (!success)
        {
            return BadRequest(new { Message = "Submission failed", Errors = errors });
        }

        return Ok(submission);
    }

    [HttpPost("submissions/{submissionId}/grade")]
    public async Task<IActionResult> GradeSubmission(int submissionId, [FromBody] GradeSubmissionRequest request)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int instructorId))
        {
            return Unauthorized("Invalid user token");
        }

        var (success, errors) = await _assignmentService.GradeSubmissionAsync(submissionId, request, instructorId);

        if (!success)
        {
            return BadRequest(new { Message = "Grading failed", Errors = errors });
        }

        return Ok(new { Message = "Submission graded successfully" });
    }

    [HttpGet("courses/{courseId}/summary")]
    public async Task<ActionResult<StudentAssignmentSummaryDTO>> GetStudentSummary(int courseId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int studentId))
        {
            return Unauthorized("Invalid user token");
        }

        var summary = await _assignmentService.GetStudentAssignmentSummaryAsync(courseId, studentId);
        return Ok(summary);
    }

    [HttpPut("submissions/{submissionId}")]
    public async Task<ActionResult<SubmissionDTO>> UpdateSubmission(int submissionId, [FromForm] SubmitAssignmentRequest request, [FromForm] List<IFormFile> files)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int studentId))
        {
            return Unauthorized("Invalid user token");
        }

        var (success, submission, errors) = await _assignmentService.UpdateSubmissionAsync(submissionId, request, files, studentId);

        if (!success)
        {
            return BadRequest(new { Message = "Submission update failed", Errors = errors });
        }

        return Ok(submission);
    }

    [HttpGet("{assignmentId}/submissions")]
    public async Task<ActionResult<List<SubmissionDTO>>> GetAssignmentSubmissions(int assignmentId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int instructorId))
        {
            return Unauthorized("Invalid user token");
        }

        var submissions = await _assignmentService.GetAssignmentSubmissionsAsync(assignmentId, instructorId);
        return Ok(submissions);
    }

    [HttpGet("{assignmentId}/submissions/my")]
    public async Task<ActionResult<SubmissionDTO>> GetMySubmission(int assignmentId)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdClaim, out int studentId))
        {
            return Unauthorized("Invalid user token");
        }

        var submission = await _assignmentService.GetStudentSubmissionAsync(assignmentId, studentId);

        if (submission == null)
        {
            return NotFound("Submission not found");
        }

        return Ok(submission);
    }

    [HttpGet("resources/{resourceId}/download")]
    public async Task<IActionResult> DownloadAssignmentResource(int resourceId)
    {
        var (success, url, error) = await _assignmentService.DownloadAssignmentResourceAsync(resourceId);

        if (!success)
        {
            return BadRequest(new { Message = error });
        }

        // For external links, return redirect
        if (url!.StartsWith("http"))
        {
            return Redirect(url);
        }

        // For S3 files, return direct URL (frontend can download directly)
        return Ok(new { DownloadUrl = url });
    }
}