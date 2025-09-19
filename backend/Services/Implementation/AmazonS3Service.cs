using Amazon.S3;
using Amazon.S3.Model;
using learnyx.Services.Interfaces;
using learnyx.Utilities.Constants;
using Microsoft.Extensions.Options;

namespace learnyx.Services.Implementation;

public class AmazonS3Service : IAmazonS3Service
{
    private readonly AwsS3Settings _settings;
    private readonly IAmazonS3 _s3Client;
    private readonly string _baseUrl;

    public AmazonS3Service(IOptions<AwsS3Settings> settings)
    {
        _settings = settings.Value;
        _baseUrl = $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/";
        _s3Client = new AmazonS3Client(
            _settings.AccessKey, 
            _settings.SecretKey,
            Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
        );
    }

    public async Task<string> UploadImageToS3(IFormFile image, string fileName)
    {
        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        
        var request = new PutObjectRequest
        {
            Key = fileName,
            BucketName = _settings.BucketName,
            ContentType = image.ContentType,
            InputStream = memoryStream,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(request);
        return $"{_baseUrl}{fileName}";
    }
    
    public async Task DeleteImageFromS3(string imageUrl)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = new Uri(imageUrl).AbsolutePath.TrimStart('/')
        };
        
        await _s3Client.DeleteObjectAsync(request);
    }
}