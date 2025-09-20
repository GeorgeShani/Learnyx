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
        var context = await _context.AssistantConversationContexts
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

        if (context == null)
        {
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
            .Select(m => new { m.TextContent, m.IsFromAssistant })
            .ToListAsync();

        // Build conversation history for Gemini
        var conversationHistory = recentMessages
            .OrderBy(m => m.IsFromAssistant) // User messages first, then assistant
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

    public async Task<List<ConversationDTO>> GetUserConversationsAsync(int userId)
    {
        var conversations = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastActivityAt)
            .ToListAsync();

        return conversations.Select(MapToConversationDTO).ToList();
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

    public async Task<ConversationDTO> CreateOrGetConversationAsync(int user1Id, int? user2Id, ConversationType type)
    {
        Conversation? conversation = null;

        if (type == ConversationType.UserToUser && user2Id.HasValue)
        {
            conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Type == ConversationType.UserToUser &&
                    ((c.User1Id == user1Id && c.User2Id == user2Id) ||
                     (c.User1Id == user2Id && c.User2Id == user1Id)));
        }
        else if (type == ConversationType.UserToAssistant)
        {
            conversation = await _context.Conversations
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
        }

        return MapToConversationDTO(conversation);
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

    private static ConversationDTO MapToConversationDTO(Conversation conversation)
    {
        return new ConversationDTO
        {
            Id = conversation.Id,
            Type = conversation.Type,
            User1Id = conversation.User1Id,
            User2Id = conversation.User2Id,
            User1Name = conversation.User1 != null ? $"{conversation.User1.FirstName} {conversation.User1.LastName}" : null,
            User2Name = conversation.User2 != null ? $"{conversation.User2.FirstName} {conversation.User2.LastName}" : null,
            User1Avatar = conversation.User1?.Avatar,
            User2Avatar = conversation.User2?.Avatar,
            LastActivityAt = conversation.LastActivityAt,
            IsActive = conversation.IsActive
        };
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
            SenderName = message.Sender != null ? $"{message.Sender.FirstName} {message.Sender.LastName}" : null,
            SenderAvatar = message.Sender?.Avatar,
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

    public async Task<ConversationDTO> GetConversationWithInfoAsync(int conversationId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
            return null;

        var DTO = MapToConversationDTO(conversation);

        // Get last message
        var lastMessage = await _context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.TextContent)
            .FirstOrDefaultAsync();

        DTO.LastMessage = lastMessage;

        return DTO;
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
}