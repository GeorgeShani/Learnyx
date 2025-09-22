namespace learnyx.Models.SignalR;

public class UserConnection
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string ConnectionId { get; set; }
    public DateTime ConnectedAt { get; set; }
}