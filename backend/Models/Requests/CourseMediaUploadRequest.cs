namespace learnyx.Models.Requests;

public class CourseMediaUploadRequest
{
    public IFormFile? Thumbnail { get; set; }
    public IFormFile? PreviewVideo { get; set; }
}