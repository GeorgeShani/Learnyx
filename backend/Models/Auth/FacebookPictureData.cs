namespace learnyx.Models.Auth;

public class FacebookPictureData
{
    public string Url { get; set; } = string.Empty;
    public bool IsSilhouette { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}