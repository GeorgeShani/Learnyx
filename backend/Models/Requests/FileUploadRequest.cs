namespace learnyx.Models.Requests;

public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
}