using learnyx.Models.DTOs;
using learnyx.Models.Requests;

namespace learnyx.Services.Interfaces;

public interface IAssignmentService
{
    Task<(bool Success, AssignmentDTO? Assignment, List<string> Errors)> CreateAssignmentAsync(int courseId, CreateAssignmentRequest request, int instructorId);
    Task<(bool Success, AssignmentDTO? Assignment, List<string> Errors)> UpdateAssignmentAsync(int assignmentId, CreateAssignmentRequest request, int instructorId);
    Task<(bool Success, List<string> Errors)> DeleteAssignmentAsync(int assignmentId, int instructorId);
    Task<AssignmentDTO?> GetAssignmentAsync(int assignmentId, int userId);
    Task<List<AssignmentDTO>> GetCourseAssignmentsAsync(int courseId, int userId);
    
    // Submission Management
    Task<(bool Success, SubmissionDTO? Submission, List<string> Errors)> SubmitAssignmentAsync(int assignmentId, SubmitAssignmentRequest request, List<IFormFile> files, int studentId);
    Task<(bool Success, SubmissionDTO? Submission, List<string> Errors)> UpdateSubmissionAsync(int submissionId, SubmitAssignmentRequest request, List<IFormFile> files, int studentId);
    Task<(bool Success, List<string> Errors)> GradeSubmissionAsync(int submissionId, GradeSubmissionRequest request, int instructorId);
    Task<List<SubmissionDTO>> GetAssignmentSubmissionsAsync(int assignmentId, int instructorId);
    Task<SubmissionDTO?> GetStudentSubmissionAsync(int assignmentId, int studentId);
    
    // Student Dashboard
    Task<StudentAssignmentSummaryDTO> GetStudentAssignmentSummaryAsync(int courseId, int studentId);
    
    // File Management
    Task<(bool Success, string? FilePath, string? Error)> DownloadSubmissionFileAsync(int fileId, int userId);
    Task<(bool Success, string? FilePath, string? Error)> DownloadAssignmentResourceAsync(int resourceId);
}