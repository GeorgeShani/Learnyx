namespace learnyx.Models.SignalR;

public class UserPresence
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public int ConnectionCount { get; set; }
}