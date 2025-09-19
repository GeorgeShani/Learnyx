namespace learnyx.Models.Entities;

public class Message : BaseEntity
{
    public int ConversationId { get; set; }
    public int? SenderId { get; set; } // Null for assistant messages
    public bool IsFromAssistant { get; set; } = false;
    public string? TextContent { get; set; } // Text part of the message
    public int? ReplyToMessageId { get; set; } // For reply functionality
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
    public User? Sender { get; set; }
    public Message? ReplyToMessage { get; set; }
    public ICollection<Message> Replies { get; set; } = new List<Message>();
    public ICollection<MessageContent> Contents { get; set; } = new List<MessageContent>();
    public ICollection<MessageReadStatus> ReadStatuses { get; set; } = new List<MessageReadStatus>();
}