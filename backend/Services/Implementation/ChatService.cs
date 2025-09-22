using learnyx.Data;
using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Enums;
using learnyx.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Services.Implementation;

public class ChatService : IChatService
{
    private readonly DataContext _context;
    private readonly IGeminiService _geminiService;

    public ChatService(DataContext context, IGeminiService geminiService)
    {
        _context = context;
        _geminiService = geminiService;
    }

    public async Task<bool> CanUserAccessConversationAsync(int conversationId, int userId)
    {
        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && 
                (c.User1Id == userId || c.User2Id == userId));
        
        return conversation != null;
    }

    public async Task<Conversation?> GetConversationAsync(int conversationId)
    {
        return await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<Message?> GetMessageAsync(int messageId)
    {
        return await _context.Messages
            .Include(m => m.Contents)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<MessageDTO> SendMessageAsync(int conversationId, int userId, string? textContent, List<MessageContentDTO> contents)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = userId,
            IsFromAssistant = false,
            TextContent = textContent
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Add message contents
        if (contents.Count != 0)
        {
            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                var messageContent = new MessageContent
                {
                    MessageId = message.Id,
                    ContentType = content.ContentType,
                    TextContent = content.TextContent,
                    FileUrl = content.FileUrl,
                    FileName = content.FileName,
                    MimeType = content.MimeType,
                    FileSize = content.FileSize,
                    Width = content.Width,
                    Height = content.Height,
                    ThumbnailUrl = content.ThumbnailUrl,
                    Order = i
                };
                _context.MessageContents.Add(messageContent);
            }
            await _context.SaveChangesAsync();
        }

        // Update conversation last activity
        await UpdateConversationActivity(conversationId);

        return await MapToMessageDTO(message);
    }

    public async Task<MessageDTO> SendAssistantMessageAsync(int conversationId, string response)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = null,
            IsFromAssistant = true,
            TextContent = response
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Update conversation last activity
        await UpdateConversationActivity(conversationId);

        return await MapToMessageDTO(message);
    }

    public async Task<string> GetAssistantResponseAsync(int conversationId)
    {
        Console.WriteLine($"GetAssistantResponseAsync called for conversation {conversationId}");
        
        var context = await _context.AssistantConversationContexts
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

        if (context == null)
        {
            Console.WriteLine("Creating new assistant context");
            context = new AssistantConversationContext
            {
                ConversationId = conversationId,
                SystemPrompt = "You are a helpful learning assistant. Provide clear, accurate, and educational responses.",
                MaxContextMessages = 10
            };
            _context.AssistantConversationContexts.Add(context);
            await _context.SaveChangesAsync();
        }

        // Get recent messages for context
        var recentMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Take(context.MaxContextMessages)
            .Select(m => new { m.TextContent, m.IsFromAssistant, m.CreatedAt })
            .ToListAsync();

        Console.WriteLine($"Found {recentMessages.Count} recent messages");

        // Build conversation history for Gemini - chronological order
        var conversationHistory = recentMessages
            .OrderBy(m => m.CreatedAt) // Chronological order for better context
            .Select(m => m.IsFromAssistant ? $"Assistant: {m.TextContent}" : $"User: {m.TextContent}")
            .ToList();

        var prompt = BuildGeminiPrompt(context.SystemPrompt, conversationHistory);
        var response = await _geminiService.AskGeminiAsync(prompt);

        // Update context
        context.LastInteractionAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return response;
    }

    public async Task MarkMessageAsReadAsync(int messageId, int userId)
    {
        var existingStatus = await _context.MessageReadStatuses
            .FirstOrDefaultAsync(s => s.MessageId == messageId && s.UserId == userId);

        if (existingStatus == null)
        {
            var status = new MessageReadStatus
            {
                MessageId = messageId,
                UserId = userId,
                Status = MessageStatus.Read,
                StatusChangedAt = DateTime.Now
            };
            _context.MessageReadStatuses.Add(status);
        }
        else
        {
            existingStatus.Status = MessageStatus.Read;
            existingStatus.StatusChangedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }

    // UPDATED: Return conversations relative to the current user
    public async Task<List<ConversationDTO?>> GetUserConversationsAsync(int userId)
    {
        var conversations = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastActivityAt)
            .ToListAsync();

        var conversationDTOs = new List<ConversationDTO?>();
        
        foreach (var conversation in conversations)
        {
            var lastMessage = await _context.Messages
                .Where(m => m.ConversationId == conversation.Id && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => m.TextContent)
                .FirstOrDefaultAsync();

            // Get unread count for this user
            var unreadCount = await GetUnreadMessageCountAsync(conversation.Id, userId);

            // Map conversation relative to the current user
            conversationDTOs.Add(MapToConversationDTORelative(conversation, userId, lastMessage, unreadCount));
        }

        return conversationDTOs;
    }

    public async Task<List<MessageDTO>> GetConversationMessagesAsync(int conversationId, int userId, int page = 1, int pageSize = 50)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Contents)
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var messageDTOs = new List<MessageDTO>();
        foreach (var message in messages)
        {
            messageDTOs.Add(await MapToMessageDTO(message));
        }

        return messageDTOs;
    }

    public async Task<ConversationDTO?> CreateOrGetConversationAsync(int user1Id, int? user2Id, ConversationType type)
    {
        Conversation? conversation = null;

        if (type == ConversationType.UserToUser && user2Id.HasValue)
        {
            conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => c.Type == ConversationType.UserToUser &&
                    ((c.User1Id == user1Id && c.User2Id == user2Id) ||
                     (c.User1Id == user2Id && c.User2Id == user1Id)));
        }
        else if (type == ConversationType.UserToAssistant)
        {
            conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => c.Type == ConversationType.UserToAssistant && c.User1Id == user1Id);
        }

        if (conversation == null)
        {
            conversation = new Conversation
            {
                Type = type,
                User1Id = user1Id,
                User2Id = user2Id,
                LastActivityAt = DateTime.Now,
                IsActive = true
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync(c => c.Id == conversation.Id);
        }

        // Return conversation relative to user1 (the requesting user)
        return MapToConversationDTORelative(conversation!, user1Id);
    }

    private async Task UpdateConversationActivity(int conversationId)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastActivityAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    private string BuildGeminiPrompt(string? systemPrompt, List<string> conversationHistory)
    {
        var prompt = systemPrompt ?? "You are a helpful assistant.";
        
        if (conversationHistory.Any())
        {
            prompt += "\n\nConversation history:\n" + string.Join("\n", conversationHistory);
        }

        return prompt;
    }

    // UPDATED: Map conversation relative to the requesting user
    private ConversationDTO MapToConversationDTORelative(Conversation conversation, int currentUserId, string? lastMessage = null, int unreadCount = 0)
    {
        var dto = new ConversationDTO
        {
            Id = conversation.Id,
            Type = conversation.Type,
            LastActivityAt = conversation.LastActivityAt,
            IsActive = conversation.IsActive,
            LastMessage = lastMessage,
            UnreadCount = unreadCount
        };

        if (conversation.Type == ConversationType.UserToAssistant)
        {
            // For assistant conversations, show assistant info
            dto.OtherUserId = null;
            dto.OtherUserName = "Learning Assistant";
            dto.OtherUserAvatar = "/assets/avatars/ai-avatar.png";
            dto.OtherUserRole = "Assistant";
            dto.IsAssistantConversation = true;
        }
        else
        {
            // For user-to-user conversations, show the OTHER user's info
            if (conversation.User1Id == currentUserId)
            {
                // Current user is User1, show User2's info
                dto.OtherUserId = conversation.User2Id;
                dto.OtherUserName = conversation.User2 != null 
                    ? $"{conversation.User2.FirstName} {conversation.User2.LastName}" 
                    : "Unknown User";
                dto.OtherUserAvatar = conversation.User2?.Avatar;
                dto.OtherUserRole = conversation.User2?.Role.ToString() ?? "Unknown";
            }
            else
            {
                // Current user is User2, show User1's info
                dto.OtherUserId = conversation.User1Id;
                dto.OtherUserName = conversation.User1 != null 
                    ? $"{conversation.User1.FirstName} {conversation.User1.LastName}" 
                    : "Unknown User";
                dto.OtherUserAvatar = conversation.User1?.Avatar;
                dto.OtherUserRole = conversation.User1?.Role.ToString() ?? "Unknown";
            }
            dto.IsAssistantConversation = false;
        }

        return dto;
    }

    // Helper method to get unread message count for a user in a conversation
    private async Task<int> GetUnreadMessageCountAsync(int conversationId, int userId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId && 
                       m.SenderId != userId &&
                       !m.IsFromAssistant &&
                       !m.IsDeleted &&
                       !_context.MessageReadStatuses.Any(rs => rs.MessageId == m.Id && 
                                                               rs.UserId == userId && 
                                                               rs.Status == MessageStatus.Read))
            .CountAsync();
    }

    private async Task<MessageDTO> MapToMessageDTO(Message message)
    {
        var contents = await _context.MessageContents
            .Where(c => c.MessageId == message.Id)
            .OrderBy(c => c.Order)
            .ToListAsync();

        return new MessageDTO
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = message.Sender != null ? $"{message.Sender.FirstName} {message.Sender.LastName}" : 
                        message.IsFromAssistant ? "Learning Assistant" : "Unknown",
            SenderAvatar = message.Sender?.Avatar ?? 
                          (message.IsFromAssistant ? "/assets/avatars/ai-avatar.png" : null),
            SenderRole = message.Sender?.Role.ToString() ?? 
                        (message.IsFromAssistant ? "Assistant" : "Unknown"),
            IsFromAssistant = message.IsFromAssistant,
            TextContent = message.TextContent,
            Contents = contents.Select(c => new MessageContentDTO
            {
                ContentType = c.ContentType,
                TextContent = c.TextContent,
                FileUrl = c.FileUrl,
                FileName = c.FileName,
                MimeType = c.MimeType,
                FileSize = c.FileSize,
                Width = c.Width,
                Height = c.Height,
                ThumbnailUrl = c.ThumbnailUrl,
                Order = c.Order
            }).ToList(),
            CreatedAt = message.CreatedAt,
            IsEdited = message.IsEdited,
            EditedAt = message.EditedAt
        };
    }
    
    public async Task<MessageDTO> EditMessageAsync(int messageId, string newContent)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Contents)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
            throw new ArgumentException("Message not found");

        message.TextContent = newContent;
        message.IsEdited = true;
        message.EditedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return await MapToMessageDTO(message);
    }

    public async Task DeleteMessageAsync(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message != null)
        {
            message.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }

    // UPDATED: Return conversation info relative to the requesting user
    public async Task<ConversationDTO?> GetConversationWithInfoAsync(int conversationId, int userId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            return null;

        // Get last message
        var lastMessage = await _context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.TextContent)
            .FirstOrDefaultAsync();

        // Get unread count
        var unreadCount = await GetUnreadMessageCountAsync(conversationId, userId);

        return MapToConversationDTORelative(conversation, userId, lastMessage, unreadCount);
    }

    public async Task<List<MessageDTO>> SearchMessagesAsync(int userId, string query, int? conversationId = null)
    {
        var messagesQuery = _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Contents)
            .Where(m => !m.IsDeleted && m.TextContent!.Contains(query));

        messagesQuery = messagesQuery.Where(m => 
            _context.Conversations.Any(c => c.Id == m.ConversationId && 
                (c.User1Id == userId || c.User2Id == userId)));

        if (conversationId.HasValue)
        {
            messagesQuery = messagesQuery.Where(m => m.ConversationId == conversationId.Value);
        }

        var messages = await messagesQuery
            .OrderByDescending(m => m.CreatedAt)
            .Take(50)
            .ToListAsync();

        var messageDTOs = new List<MessageDTO>();
        foreach (var message in messages)
        {
            messageDTOs.Add(await MapToMessageDTO(message));
        }

        return messageDTOs;
    }

    public async Task MarkAllMessagesAsReadAsync(int conversationId, int userId)
    {
        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId && 
                       m.SenderId != userId && 
                       !m.IsDeleted)
            .Select(m => m.Id)
            .ToListAsync();

        foreach (var messageId in unreadMessages)
        {
            await MarkMessageAsReadAsync(messageId, userId);
        }
    }
}
