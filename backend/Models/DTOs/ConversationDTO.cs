using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class ConversationDTO
{
    public int Id { get; set; }
    public ConversationType Type { get; set; }
    public DateTime LastActivityAt { get; set; }
    public bool IsActive { get; set; }
    public string? LastMessage { get; set; }
    public int UnreadCount { get; set; }
        
    // Relative fields - shows the OTHER person in the conversation
    public int? OtherUserId { get; set; }
    public string? OtherUserName { get; set; }
    public string? OtherUserAvatar { get; set; }
    public string? OtherUserRole { get; set; }
    public bool IsAssistantConversation { get; set; }
}