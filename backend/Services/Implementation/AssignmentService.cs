using learnyx.Data;
using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Enums;
using learnyx.Models.Requests;
using learnyx.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Services.Implementation;

public class AssignmentService : IAssignmentService
{
    private readonly DataContext _context;
    private readonly IAmazonS3Service _s3Service;
    private readonly ILogger<AssignmentService> _logger;
    private const int MaxFilesPerSubmission = 10;

    public AssignmentService(
        DataContext context,
        IAmazonS3Service s3Service,
        ILogger<AssignmentService> logger
    ) {
        _context = context;
        _s3Service = s3Service;
        _logger = logger;
    }
    
    public async Task<(bool Success, AssignmentDTO? Assignment, List<string> Errors)> CreateAssignmentAsync(int courseId, CreateAssignmentRequest request, int instructorId)
    {
        try
        {
            var errors = new List<string>();
            
            // Verify instructor owns the course
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.TeacherId == instructorId);
            if (course == null)
            {
                errors.Add("Course not found or you don't have permission to create assignments");
                return (false, null, errors);
            }
            
            // Validate due date
            if (request.DueDate <= DateTime.UtcNow)
            {
                errors.Add("Due date must be in the future");
                return (false, null, errors);
            }
            
            // Create assignment
            var assignment = new Assignment
            {
                Title = request.Title,
                Description = request.Description,
                Instructions = request.Instructions,
                DueDate = request.DueDate,
                MaxPoints = request.MaxPoints,
                SubmissionType = request.SubmissionType,
                AllowLateSubmission = request.AllowLateSubmission,
                LatePenaltyPercentage = request.LatePenaltyPercentage,
                IsVisible = request.IsVisible,
                Order = request.Order,
                CourseId = courseId,
                LessonId = request.LessonId
            };
            
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            
            // Create resources
            foreach (var resource in request.Resources
                     .Select(resourceRequest => new AssignmentResource
                     {
                         Name = resourceRequest.Name,
                         Type = resourceRequest.Type,
                         ExternalUrl = resourceRequest.ExternalUrl,
                         Description = resourceRequest.Description,
                         Order = resourceRequest.Order,
                         IsRequired = resourceRequest.IsRequired,
                         AssignmentId = assignment.Id
                     }
                )
            ) {
                _context.AssignmentResources.Add(resource);
            }
            
            await _context.SaveChangesAsync();
            
            var assignmentDTO = await MapToAssignmentDTO(assignment, null);
            return (true, assignmentDTO, new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating assignment for course {CourseId}", courseId);
            return (false, null, new List<string> { "An error occurred while creating the assignment" });
        }
    }

    public async Task<(bool Success, SubmissionDTO? Submission, List<string> Errors)> SubmitAssignmentAsync(int assignmentId, SubmitAssignmentRequest request, List<IFormFile> files, int studentId)
    {
        try
        {
            var errors = new List<string>();
            
            // Validate file count
            if (files.Count > MaxFilesPerSubmission)
            {
                errors.Add($"Maximum {MaxFilesPerSubmission} files allowed per submission");
                return (false, null, errors);
            }
            
            // Get assignment and verify it exists
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);
                
            if (assignment == null)
            {
                errors.Add("Assignment not found");
                return (false, null, errors);
            }
            
            // Verify a student is enrolled in the course
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == assignment.CourseId && e.StudentId == studentId);
                
            if (enrollment == null)
            {
                errors.Add("You are not enrolled in this course");
                return (false, null, errors);
            }
            
            // Check if submission already exists
            var existingSubmission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
                
            if (existingSubmission != null)
            {
                errors.Add("Submission already exists. Use update instead.");
                return (false, null, errors);
            }
            
            // Calculate if late
            var isLate = DateTime.UtcNow > assignment.DueDate;
            var daysLate = isLate ? (int)(DateTime.UtcNow - assignment.DueDate).TotalDays : 0;
            
            // Check if late submissions are allowed
            if (isLate && !assignment.AllowLateSubmission)
            {
                errors.Add("Late submissions are not allowed for this assignment");
                return (false, null, errors);
            }
            
            // Validate submission type requirements
            if (assignment.SubmissionType == SubmissionType.Text && string.IsNullOrWhiteSpace(request.TextContent))
            {
                errors.Add("Text content is required for this assignment");
                return (false, null, errors);
            }
            
            if (assignment.SubmissionType == SubmissionType.File && !files.Any())
            {
                errors.Add("File upload is required for this assignment");
                return (false, null, errors);
            }
            
            // Create submission
            var submission = new Submission
            {
                AssignmentId = assignmentId,
                StudentId = studentId,
                TextContent = request.TextContent,
                Status = AssignmentStatus.Submitted,
                SubmittedAt = DateTime.UtcNow,
                IsLate = isLate,
                DaysLate = daysLate
            };
            
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();
            
            // Handle file uploads
            if (files.Any())
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    
                    try
                    {
                        // Generate unique filename for S3
                        var fileExtension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"submissions/{Guid.NewGuid()}{fileExtension}";
                        
                        // Upload to S3
                        var s3Url = await _s3Service.UploadImageToS3(file, uniqueFileName);
                        
                        var submissionFile = new SubmissionFile
                        {
                            OriginalFileName = file.FileName,
                            StoredFileName = uniqueFileName,
                            FilePath = s3Url,
                            MimeType = file.ContentType,
                            FileSize = file.Length,
                            Order = i + 1,
                            SubmissionId = submission.Id
                        };
                        
                        _context.SubmissionFiles.Add(submissionFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to upload file {FileName} for submission {SubmissionId}", file.FileName, submission.Id);
                        errors.Add($"Failed to upload file {file.FileName}");
                    }
                }
                
                await _context.SaveChangesAsync();
            }
            
            // Load submission with related data for response
            var submissionWithData = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .Include(s => s.Files)
                .FirstAsync(s => s.Id == submission.Id);
            
            var submissionDTO = MapToSubmissionDTO(submissionWithData);
            return (true, submissionDTO, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting assignment {AssignmentId} for student {StudentId}", assignmentId, studentId);
            return (false, null, new List<string> { "An error occurred while submitting the assignment" });
        }
    }

    public async Task<(bool Success, List<string> Errors)> GradeSubmissionAsync(int submissionId, GradeSubmissionRequest request, int instructorId)
    {
        try
        {
            var errors = new List<string>();
            
            // Get submission with assignment and course info
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
                .FirstOrDefaultAsync(s => s.Id == submissionId);
                
            if (submission == null)
            {
                errors.Add("Submission not found");
                return (false, errors);
            }
            
            // Verify instructor owns the course
            if (submission.Assignment.Course.TeacherId != instructorId)
            {
                errors.Add("You don't have permission to grade this submission");
                return (false, errors);
            }
            
            // Validate grade
            if (request.Grade > submission.Assignment.MaxPoints)
            {
                errors.Add($"Grade cannot exceed maximum points ({submission.Assignment.MaxPoints})");
                return (false, errors);
            }
            
            // Apply late penalty if applicable
            var finalGrade = request.Grade;
            if (submission is { IsLate: true, Assignment.LatePenaltyPercentage: > 0 })
            {
                var penalty = (finalGrade * submission.Assignment.LatePenaltyPercentage / 100.0) * submission.DaysLate;
                finalGrade = Math.Max(0, finalGrade - (int)penalty);
            }
            
            // Update submission
            submission.Grade = finalGrade;
            submission.Feedback = request.Feedback;
            submission.Status = AssignmentStatus.Graded;
            submission.GradedAt = DateTime.UtcNow;
            submission.GradedById = instructorId;
            
            await _context.SaveChangesAsync();
            
            return (true, new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error grading submission {SubmissionId}", submissionId);
            return (false, new List<string> { "An error occurred while grading the submission" });
        }
    }

    public async Task<StudentAssignmentSummaryDTO> GetStudentAssignmentSummaryAsync(int courseId, int studentId)
    {
        try
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                return new StudentAssignmentSummaryDTO();
            }
            
            var assignments = await _context.Assignments
                .Include(a => a.Submissions.Where(s => s.StudentId == studentId))
                .Where(a => a.CourseId == courseId && a.IsVisible)
                .OrderBy(a => a.Order)
                .ToListAsync();
            
            var submissions = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Files)
                .Where(s => s.Assignment.CourseId == courseId && s.StudentId == studentId)
                .OrderByDescending(s => s.SubmittedAt)
                .Take(5)
                .ToListAsync();
            
            var gradedSubmissions = submissions.Where(s => s.Grade.HasValue).ToList();
            var averageGrade = gradedSubmissions.Any() 
                ? gradedSubmissions.Average(s => (double)s.Grade!.Value / s.Assignment.MaxPoints * 100)
                : 0.0;
            
            var upcomingAssignments = assignments
                .Where(a => !a.Submissions.Any() && a.DueDate > DateTime.UtcNow)
                .Take(3)
                .Select(a => MapToAssignmentDTO(a, null).Result)
                .ToList();
            
            return new StudentAssignmentSummaryDTO
            {
                CourseId = courseId,
                CourseTitle = course.Title,
                TotalAssignments = assignments.Count,
                CompletedAssignments = assignments.Count(a => a.Submissions.Any(s => s.Status == AssignmentStatus.Graded)),
                PendingAssignments = assignments.Count(a => !a.Submissions.Any() || a.Submissions.Any(s => s.Status == AssignmentStatus.Pending || s.Status == AssignmentStatus.Submitted)),
                GradedAssignments = gradedSubmissions.Count,
                AverageGrade = averageGrade,
                UpcomingAssignments = upcomingAssignments,
                RecentSubmissions = submissions.Select(MapToSubmissionDTO).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignment summary for student {StudentId} in course {CourseId}", studentId, courseId);
            return new StudentAssignmentSummaryDTO();
        }
    }

    // ===== HELPER METHODS =====

    private async Task<AssignmentDTO> MapToAssignmentDTO(Assignment assignment, int? studentId)
    {
        var DTO = new AssignmentDTO
        {
            Id = assignment.Id,
            Title = assignment.Title,
            Description = assignment.Description,
            Instructions = assignment.Instructions,
            DueDate = assignment.DueDate,
            MaxPoints = assignment.MaxPoints,
            SubmissionType = assignment.SubmissionType,
            AllowLateSubmission = assignment.AllowLateSubmission,
            LatePenaltyPercentage = assignment.LatePenaltyPercentage,
            IsVisible = assignment.IsVisible,
            Order = assignment.Order,
            LessonId = assignment.LessonId,
            CreatedAt = assignment.CreatedAt,
            UpdatedAt = assignment.UpdatedAt
        };
        
        // Load resources if not already loaded
        if (assignment.Resources.Count == 0)
        {
            assignment.Resources = await _context.AssignmentResources
                .Where(r => r.AssignmentId == assignment.Id)
                .OrderBy(r => r.Order)
                .ToListAsync();
        }
        
        DTO.Resources = assignment.Resources.Select(r => new AssignmentResourceDTO
        {
            Id = r.Id,
            Name = r.Name,
            Type = r.Type,
            ExternalUrl = r.ExternalUrl,
            DownloadUrl = r.Type != ResourceType.Link ? $"/api/assignments/resources/{r.Id}/download" : null,
            SizeDisplay = r.GetSizeDisplay(),
            Description = r.Description,
            Order = r.Order,
            IsRequired = r.IsRequired
        }).ToList();
        
        // Load student submission if studentId provided
        if (studentId.HasValue)
        {
            var submission = await _context.Submissions
                .Include(s => s.Files)
                .FirstOrDefaultAsync(s => s.AssignmentId == assignment.Id && s.StudentId == studentId.Value);
                
            if (submission != null)
            {
                DTO.StudentSubmission = MapToSubmissionDTO(submission);
            }
        }
        
        return DTO;
    }

    private static SubmissionDTO MapToSubmissionDTO(Submission submission)
    {
        return new SubmissionDTO
        {
            Id = submission.Id,
            TextContent = submission.TextContent,
            Status = submission.Status,
            SubmittedAt = submission.SubmittedAt,
            GradedAt = submission.GradedAt,
            Grade = submission.Grade,
            Feedback = submission.Feedback,
            IsLate = submission.IsLate,
            DaysLate = submission.DaysLate,
            StudentId = submission.StudentId,
            StudentName = $"{submission.Student.FirstName} {submission.Student.LastName}",
            StudentEmail = submission.Student.Email,
            AssignmentId = submission.AssignmentId,
            AssignmentTitle = submission.Assignment.Title,
            MaxPoints = submission.Assignment.MaxPoints,
            Files = submission.Files.Select(f => new SubmissionFileDTO
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                MimeType = f.MimeType,
                FileSize = f.FileSize,
                Description = f.Description,
                Order = f.Order,
                DownloadUrl = $"/api/assignments/submissions/files/{f.Id}/download",
                FileSizeFormatted = f.GetFileSizeFormatted()
            }).OrderBy(f => f.Order).ToList()
        };
    }

    // Implement remaining interface methods with full implementation
    public async Task<(bool Success, AssignmentDTO? Assignment, List<string> Errors)> UpdateAssignmentAsync(int assignmentId, CreateAssignmentRequest request, int instructorId)
    {
        try
        {
            var errors = new List<string>();
            
            // Get assignment with course info
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Resources)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);
                
            if (assignment == null)
            {
                errors.Add("Assignment not found");
                return (false, null, errors);
            }
            
            // Verify instructor owns the course
            if (assignment.Course.TeacherId != instructorId)
            {
                errors.Add("You don't have permission to update this assignment");
                return (false, null, errors);
            }
            
            // Check if assignment has submissions - if so, limit what can be changed
            var hasSubmissions = await _context.Submissions.AnyAsync(s => s.AssignmentId == assignmentId);
            
            if (hasSubmissions)
            {
                // Only allow changes to title, description, instructions, and visibility
                assignment.Title = request.Title;
                assignment.Description = request.Description;
                assignment.Instructions = request.Instructions;
                assignment.IsVisible = request.IsVisible;
            }
            else
            {
                // Allow all changes if no submissions yet
                assignment.Title = request.Title;
                assignment.Description = request.Description;
                assignment.Instructions = request.Instructions;
                assignment.DueDate = request.DueDate;
                assignment.MaxPoints = request.MaxPoints;
                assignment.SubmissionType = request.SubmissionType;
                assignment.AllowLateSubmission = request.AllowLateSubmission;
                assignment.LatePenaltyPercentage = request.LatePenaltyPercentage;
                assignment.IsVisible = request.IsVisible;
                assignment.Order = request.Order;
                assignment.LessonId = request.LessonId;
                
                // Update resources (remove old ones, add new ones)
                _context.AssignmentResources.RemoveRange(assignment.Resources);
                
                foreach (var resourceRequest in request.Resources)
                {
                    var resource = new AssignmentResource
                    {
                        Name = resourceRequest.Name,
                        Type = resourceRequest.Type,
                        ExternalUrl = resourceRequest.ExternalUrl,
                        Description = resourceRequest.Description,
                        Order = resourceRequest.Order,
                        IsRequired = resourceRequest.IsRequired,
                        AssignmentId = assignment.Id
                    };
                    
                    _context.AssignmentResources.Add(resource);
                }
            }
            
            await _context.SaveChangesAsync();
            
            var assignmentDTO = await MapToAssignmentDTO(assignment, null);
            return (true, assignmentDTO, new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating assignment {AssignmentId}", assignmentId);
            return (false, null, new List<string> { "An error occurred while updating the assignment" });
        }
    }

    public async Task<(bool Success, List<string> Errors)> DeleteAssignmentAsync(int assignmentId, int instructorId)
    {
        try
        {
            var errors = new List<string>();
            
            // Get assignment with course and submissions info
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Submissions)
                .ThenInclude(s => s.Files)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);
                
            if (assignment == null)
            {
                errors.Add("Assignment not found");
                return (false, errors);
            }
            
            // Verify instructor owns the course
            if (assignment.Course.TeacherId != instructorId)
            {
                errors.Add("You don't have permission to delete this assignment");
                return (false, errors);
            }
            
            // Delete files from S3 for all submissions
            foreach (var submission in assignment.Submissions)
            {
                foreach (var file in submission.Files)
                {
                    try
                    {
                        await _s3Service.DeleteImageFromS3(file.FilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete file from S3: {FilePath}", file.FilePath);
                        // Continue with deletion even if S3 cleanup fails
                    }
                }
            }
            
            // Delete assignment (cascade will handle submissions and files)
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            
            return (true, new List<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting assignment {AssignmentId}", assignmentId);
            return (false, new List<string> { "An error occurred while deleting the assignment" });
        }
    }

    public async Task<AssignmentDTO?> GetAssignmentAsync(int assignmentId, int userId)
    {
        try
        {
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Lesson)
                .Include(a => a.Resources)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);
                
            if (assignment == null)
            {
                return null;
            }
            
            // Check permissions - must be enrolled student or course instructor
            var isInstructor = assignment.Course.TeacherId == userId;
            var isEnrolledStudent = await _context.CourseEnrollments
                .AnyAsync(e => e.CourseId == assignment.CourseId && e.StudentId == userId);
                
            if (!isInstructor && !isEnrolledStudent)
            {
                return null;
            }
            
            // Only show visible assignments to students
            if (!isInstructor && !assignment.IsVisible)
            {
                return null;
            }
            
            var studentId = isInstructor ? null : (int?)userId;
            return await MapToAssignmentDTO(assignment, studentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignment {AssignmentId}", assignmentId);
            return null;
        }
    }

    public async Task<List<AssignmentDTO>> GetCourseAssignmentsAsync(int courseId, int userId)
    {
        try
        {
            // Check if user is instructor or enrolled student
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                return new List<AssignmentDTO>();
            }
            
            var isInstructor = course.TeacherId == userId;
            var isEnrolledStudent = await _context.CourseEnrollments
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == userId);
                
            if (!isInstructor && !isEnrolledStudent)
            {
                return new List<AssignmentDTO>();
            }
            
            var query = _context.Assignments
                .Include(a => a.Resources)
                .Where(a => a.CourseId == courseId);
                
            // Students only see visible assignments
            if (!isInstructor)
            {
                query = query.Where(a => a.IsVisible);
            }
            
            var assignments = await query
                .OrderBy(a => a.Order)
                .ThenBy(a => a.DueDate)
                .ToListAsync();
            
            var result = new List<AssignmentDTO>();
            var studentId = isInstructor ? null : (int?)userId;
            
            foreach (var assignment in assignments)
            {
                var DTO = await MapToAssignmentDTO(assignment, studentId);
                result.Add(DTO);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignments for course {CourseId}", courseId);
            return new List<AssignmentDTO>();
        }
    }

    public async Task<(bool Success, SubmissionDTO? Submission, List<string> Errors)> UpdateSubmissionAsync(int submissionId, SubmitAssignmentRequest request, List<IFormFile> files, int studentId)
    {
        try
        {
            var errors = new List<string>();
            
            // Get existing submission with assignment and files
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Files)
                .FirstOrDefaultAsync(s => s.Id == submissionId && s.StudentId == studentId);
                
            if (submission == null)
            {
                errors.Add("Submission not found or you don't have permission to update it");
                return (false, null, errors);
            }
            
            // Check if the submission can be updated (not graded yet)
            if (submission.Status == AssignmentStatus.Graded)
            {
                errors.Add("Cannot update a graded submission");
                return (false, null, errors);
            }
            
            // Validate file count (existing plus new files)
            if (submission.Files.Count + files.Count > MaxFilesPerSubmission)
            {
                errors.Add($"Maximum {MaxFilesPerSubmission} files allowed per submission");
                return (false, null, errors);
            }
            
            // Update text content
            submission.TextContent = request.TextContent;
            
            // Handle new file uploads
            if (files.Any())
            {
                var nextOrder = submission.Files.Any() ? submission.Files.Max(f => f.Order) + 1 : 1;
                
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    
                    try
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"submissions/{Guid.NewGuid()}{fileExtension}";
                        
                        var s3Url = await _s3Service.UploadImageToS3(file, uniqueFileName);
                        
                        var submissionFile = new SubmissionFile
                        {
                            OriginalFileName = file.FileName,
                            StoredFileName = uniqueFileName,
                            FilePath = s3Url,
                            MimeType = file.ContentType,
                            FileSize = file.Length,
                            Order = nextOrder + i,
                            SubmissionId = submission.Id
                        };
                        
                        _context.SubmissionFiles.Add(submissionFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to upload file {FileName} for submission {SubmissionId}", file.FileName, submission.Id);
                        errors.Add($"Failed to upload file {file.FileName}");
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            // Reload submission with updated data
            var updatedSubmission = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .Include(s => s.Files)
                .FirstAsync(s => s.Id == submission.Id);
            
            var submissionDTO = MapToSubmissionDTO(updatedSubmission);
            return (true, submissionDTO, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating submission {SubmissionId}", submissionId);
            return (false, null, new List<string> { "An error occurred while updating the submission" });
        }
    }

    public async Task<List<SubmissionDTO>> GetAssignmentSubmissionsAsync(int assignmentId, int instructorId)
    {
        try
        {
            // Verify instructor owns the course
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);
                
            if (assignment == null || assignment.Course.TeacherId != instructorId)
            {
                return new List<SubmissionDTO>();
            }
            
            var submissions = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .Include(s => s.Files)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
            
            return submissions.Select(MapToSubmissionDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submissions for assignment {AssignmentId}", assignmentId);
            return new List<SubmissionDTO>();
        }
    }

    public async Task<SubmissionDTO?> GetStudentSubmissionAsync(int assignmentId, int studentId)
    {
        try
        {
            // Verify student is enrolled in the course
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);
                
            if (assignment == null)
            {
                return null;
            }
            
            var isEnrolled = await _context.CourseEnrollments
                .AnyAsync(e => e.CourseId == assignment.CourseId && e.StudentId == studentId);
                
            if (!isEnrolled)
            {
                return null;
            }
            
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .Include(s => s.Files)
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
                
            return submission != null ? MapToSubmissionDTO(submission) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting submission for assignment {AssignmentId} and student {StudentId}", assignmentId, studentId);
            return null;
        }
    }

    public async Task<(bool Success, string? FilePath, string? Error)> DownloadSubmissionFileAsync(int fileId, int userId)
    {
        try
        {
            var file = await _context.SubmissionFiles
                .Include(f => f.Submission)
                .ThenInclude(s => s.Assignment)
                .ThenInclude(a => a.Course)
                .FirstOrDefaultAsync(f => f.Id == fileId);
                
            if (file == null)
            {
                return (false, null, "File not found");
            }
            
            // Check permissions - student can download their own files, instructor can download all files in their courses
            var isStudent = file.Submission.StudentId == userId;
            var isInstructor = file.Submission.Assignment.Course.TeacherId == userId;
            
            if (!isStudent && !isInstructor)
            {
                return (false, null, "You don't have permission to download this file");
            }
            
            // Return S3 URL - frontend can download directly from S3
            return (true, file.FilePath, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting download URL for submission file {FileId}", fileId);
            return (false, null, "An error occurred while getting the download URL");
        }
    }

    public async Task<(bool Success, string? FilePath, string? Error)> DownloadAssignmentResourceAsync(int resourceId)
    {
        try
        {
            var resource = await _context.AssignmentResources
                .Include(r => r.Assignment)
                .ThenInclude(a => a.Course)
                .FirstOrDefaultAsync(r => r.Id == resourceId);
                
            if (resource == null)
            {
                return (false, null, "Resource not found");
            }
            
            // For links, return the external URL
            if (resource.Type == ResourceType.Link)
            {
                return (true, resource.ExternalUrl, null);
            }
            
            // For files, return the file path (could be S3 URL or local path)
            return (true, resource.FilePath, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting download URL for assignment resource {ResourceId}", resourceId);
            return (false, null, "An error occurred while getting the download URL");
        }
    }
}