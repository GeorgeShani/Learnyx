namespace learnyx.Models.DTOs;

public class StudentAssignmentSummaryDTO
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int TotalAssignments { get; set; }
    public int CompletedAssignments { get; set; }
    public int PendingAssignments { get; set; }
    public int GradedAssignments { get; set; }
    public double AverageGrade { get; set; }
    public List<AssignmentDTO> UpcomingAssignments { get; set; } = new();
    public List<SubmissionDTO> RecentSubmissions { get; set; } = new();
}