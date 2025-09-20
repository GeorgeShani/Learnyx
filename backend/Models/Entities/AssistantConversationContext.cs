namespace learnyx.Models.Entities;

public class AssistantConversationContext : BaseEntity
{
    public string? SystemPrompt { get; set; }
    public int MaxContextMessages { get; set; } = 10;
    public DateTime LastInteractionAt { get; set; }
    
    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;
}