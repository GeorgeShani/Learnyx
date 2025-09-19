using learnyx.Models.Enums;

namespace learnyx.Models.Entities;

public class Conversation : BaseEntity
{
    public ConversationType Type { get; set; }
    public int? User1Id { get; set; } // First participant
    public int? User2Id { get; set; } // Second participant (null for assistant conversations)
    public DateTime LastActivityAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User? User1 { get; set; }
    public User? User2 { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}