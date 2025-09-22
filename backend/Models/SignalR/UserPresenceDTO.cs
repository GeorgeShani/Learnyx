namespace learnyx.Models.SignalR;

public class UserPresenceDTO
{
    public int UserId { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}