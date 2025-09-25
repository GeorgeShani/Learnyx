using System.ComponentModel.DataAnnotations;
using learnyx.Models.Enums;

namespace learnyx.Models.Requests;

public class CreateAssignmentRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(4000)]
    public string Instructions { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Range(1, 1000)]
    public int MaxPoints { get; set; } = 100;

    public SubmissionType SubmissionType { get; set; } = SubmissionType.File;

    public bool AllowLateSubmission { get; set; } = true;

    [Range(0, 100)]
    public int LatePenaltyPercentage { get; set; } = 10;

    public bool IsVisible { get; set; } = true;

    public int Order { get; set; }

    public int? LessonId { get; set; }

    public List<CreateAssignmentResourceRequest> Resources { get; set; } = new();
}