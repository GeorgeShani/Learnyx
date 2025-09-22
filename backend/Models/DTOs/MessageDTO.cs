namespace learnyx.Models.DTOs;

public class MessageDTO
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int? SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderAvatar { get; set; }
    public string? SenderRole { get; set; }
    public bool IsFromAssistant { get; set; }
    public string? TextContent { get; set; }
    public List<MessageContentDTO> Contents { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
}