using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class MessageReadStatus : BaseEntity
{
    public MessageStatus Status { get; set; }
    public DateTime StatusChangedAt { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;
}