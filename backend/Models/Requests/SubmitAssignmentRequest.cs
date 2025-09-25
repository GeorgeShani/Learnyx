using System.ComponentModel.DataAnnotations;

namespace learnyx.Models.Requests;

public class SubmitAssignmentRequest
{
    public int AssignmentId { get; set; }

    [StringLength(4000)]
    public string? TextContent { get; set; }
}