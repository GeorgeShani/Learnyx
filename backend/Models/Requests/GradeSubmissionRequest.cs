using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class GradeSubmissionRequest
{
    [Range(0, int.MaxValue)]
    public int Grade { get; set; }

    [StringLength(2000)]
    public string? Feedback { get; set; }
}