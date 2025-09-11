namespace learnyx.Models.Auth;

public class FacebookTokenData
{
    public string AppId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Application { get; set; } = string.Empty;
    public long DataAccessExpiresAt { get; set; }
    public long ExpiresAt { get; set; }
    public bool IsValid { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
}