namespace learnyx.Models.Requests;

public class CourseSettingsRequest
{
    public bool EnableQA { get; set; } = false;
    public bool EnableReviews { get; set; } = true;
    public bool EnableDiscussions { get; set; } = false;
    public bool AutoApprove { get; set; } = true;
    public bool IssueCertificates { get; set; } = true;
    public bool SendCompletionEmails { get; set; } = false;
}