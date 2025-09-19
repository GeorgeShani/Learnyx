namespace learnyx.Services.Interfaces;

public interface IAmazonS3Service
{
    Task<string> UploadImageToS3(IFormFile image, string fileName);
    Task DeleteImageFromS3(string imageUrl);
}