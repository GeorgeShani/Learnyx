using learnyx.Models.Enums;

namespace learnyx.Models.DTOs;

public class ConversationDTO
{
    public int Id { get; set; }
    public ConversationType Type { get; set; }
    public int? User1Id { get; set; }
    public int? User2Id { get; set; }
    public string? User1Name { get; set; }
    public string? User2Name { get; set; }
    public string? User1Avatar { get; set; }
    public string? User2Avatar { get; set; }
    public DateTime LastActivityAt { get; set; }
    public bool IsActive { get; set; }
    public string? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}