using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Enums;

namespace learnyx.Services.Interfaces;

public interface IChatService
{
    Task<bool> CanUserAccessConversationAsync(int conversationId, int userId);
    Task<Conversation?> GetConversationAsync(int conversationId);
    Task<Message?> GetMessageAsync(int messageId);
    Task<MessageDTO> SendMessageAsync(int conversationId, int userId, string? textContent, List<MessageContentDTO> contents);
    Task<MessageDTO> SendAssistantMessageAsync(int conversationId, string response);
    Task<string> GetAssistantResponseAsync(int conversationId);
    Task<List<ConversationDTO>> GetUserConversationsAsync(int userId);
    Task<List<MessageDTO>> GetConversationMessagesAsync(int conversationId, int userId, int page = 1, int pageSize = 50);
    Task<ConversationDTO> CreateOrGetConversationAsync(int user1Id, int? user2Id, ConversationType type);
    Task<MessageDTO> EditMessageAsync(int messageId, string newContent);
    Task DeleteMessageAsync(int messageId);
    Task<ConversationDTO> GetConversationWithInfoAsync(int conversationId);
    Task<List<MessageDTO>> SearchMessagesAsync(int userId, string query, int? conversationId = null);
}