using learnyx.Hubs;
using learnyx.Models.DTOs;
using learnyx.Models.Enums;
using System.Security.Claims;
using learnyx.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace learnyx.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IAmazonS3Service _amazonS3Service;

    public ChatController(
        IChatService chatService, 
        IHubContext<ChatHub> hubContext,
        IAmazonS3Service amazonS3Service
    ) {
        _chatService = chatService;
        _hubContext = hubContext;
        _amazonS3Service = amazonS3Service;
    }

    // GET: api/chat/conversations
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDTO>>> GetUserConversations()
    {
        try
        {
            var userId = GetCurrentUserId();
            var conversations = await _chatService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving conversations: {ex.Message}");
        }
    }

    // GET: api/chat/conversations/{id}/messages
    [HttpGet("conversations/{id:int}/messages")]
    public async Task<ActionResult<List<MessageDTO>>> GetConversationMessages(
        int id, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 50
    ) {
        try
        {
            var userId = GetCurrentUserId();
            
            if (!await _chatService.CanUserAccessConversationAsync(id, userId))
            {
                return Forbid("You don't have access to this conversation");
            }

            var messages = await _chatService.GetConversationMessagesAsync(id, userId, page, pageSize);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving messages: {ex.Message}");
        }
    }

    // POST: api/chat/conversations
    [HttpPost("conversations")]
    public async Task<ActionResult<ConversationDTO>> CreateConversation([FromBody] CreateConversationRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (request is { Type: ConversationType.UserToUser, User2Id: null })
            {
                return BadRequest("User2Id is required for user-to-user conversations");
            }

            if (request.Type == ConversationType.UserToUser && request.User2Id == userId)
            {
                return BadRequest("Cannot create conversation with yourself");
            }

            var conversation = await _chatService.CreateOrGetConversationAsync(
                userId, request.User2Id, request.Type);

            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating conversation: {ex.Message}");
        }
    }

    // POST: api/chat/conversations/{id}/messages
    [HttpPost("conversations/{id}/messages")]
    public async Task<ActionResult<MessageDTO>> SendMessage(int id, [FromBody] SendMessageRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
    
            if (!await _chatService.CanUserAccessConversationAsync(id, userId))
            {
                return Forbid("You don't have access to this conversation");
            }
    
            if (string.IsNullOrEmpty(request.TextContent) && request.Contents?.Any() != true)
            {
                return BadRequest("Message must have text content or attachments");
            }
    
            var message = await _chatService.SendMessageAsync(id, userId, request.TextContent, request.Contents ?? new List<MessageContentDTO>());
    
            // Notify other users via SignalR
            await _hubContext.Clients.Group($"conversation_{id}")
                .SendAsync("ReceiveMessage", message);
    
            var conversation = await _chatService.GetConversationAsync(id);
            if (conversation?.Type == ConversationType.UserToAssistant)
            {
                await _hubContext.Clients.Group($"conversation_{id}")
                    .SendAsync("AssistantTyping", true);
    
                _ = ProcessAssistantResponseInBackground(id);
            }
    
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error sending message: {ex.Message}");
        }
    }

    private async Task ProcessAssistantResponseInBackground(int conversationId)
    {
        try
        {
            // Small delay to let the user message appear first
            await Task.Delay(1000);

            var response = await _chatService.GetAssistantResponseAsync(conversationId);

            if (!string.IsNullOrEmpty(response))
            {
                var assistantMessage = await _chatService.SendAssistantMessageAsync(conversationId, response);
                
                await _hubContext.Clients.Group($"conversation_{conversationId}")
                    .SendAsync("ReceiveMessage", assistantMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Background assistant response error: {ex}");
            
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("Error", $"Assistant temporarily unavailable: {ex.Message}");
        }
        finally
        {
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("AssistantTyping", false);
        }
    }

    // POST: api/chat/messages/upload
    [HttpPost("messages/upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<MessageContentDTO>> UploadFile([FromForm] FileUploadRequest request)
    {
        try
        {
            var file = request.File;
            if (file.Length == 0)
            {
                return BadRequest("No file provided");
            }
    
            // Validate file type and size
            var allowedTypes = new[] 
            { 
                "image/jpeg", "image/png", "image/gif", "image/webp",                      
                "application/pdf", "text/plain", "application/msword", 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" 
            };
            
            if (!allowedTypes.Contains(file.ContentType))
            {
                return BadRequest("File type not allowed");
            }
    
            if (file.Length > 10 * 1024 * 1024) // 10MB limit
            {
                return BadRequest("File size exceeds limit (10MB)");
            }
    
            var imageUrl = await _amazonS3Service.UploadImageToS3(file, file.FileName);
            
            var contentType = file.ContentType.StartsWith("image/") 
                ? MessageContentType.Image 
                : MessageContentType.File;
    
            var messageContent = new MessageContentDTO
            {
                ContentType = contentType,
                FileUrl = imageUrl,
                FileName = file.FileName,
                MimeType = file.ContentType,
                FileSize = file.Length
            };
    
            return Ok(messageContent);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error uploading file: {ex.Message}");
        }
    }

    // DELETE: api/chat/messages/{id}
    [HttpDelete("messages/{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _chatService.GetMessageAsync(id);

            if (message == null)
            {
                return NotFound("Message not found");
            }

            if (message.SenderId != userId)
            {
                return Forbid("You can only delete your own messages");
            }

            await _chatService.DeleteMessageAsync(id);

            await _hubContext.Clients.Group($"conversation_{message.ConversationId}")
                .SendAsync("MessageDeleted", id);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error deleting message: {ex.Message}");
        }
    }

    // PUT: api/chat/messages/{id}
    [HttpPut("messages/{id}")]
    public async Task<ActionResult<MessageDTO>> EditMessage(int id, [FromBody] EditMessageRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _chatService.GetMessageAsync(id);

            if (message == null)
            {
                return NotFound("Message not found");
            }

            if (message.SenderId != userId)
            {
                return Forbid("You can only edit your own messages");
            }

            if (message.IsFromAssistant)
            {
                return BadRequest("Cannot edit assistant messages");
            }

            var updatedMessage = await _chatService.EditMessageAsync(id, request.TextContent);

            await _hubContext.Clients.Group($"conversation_{message.ConversationId}")
                .SendAsync("MessageEdited", updatedMessage);

            return Ok(updatedMessage);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error editing message: {ex.Message}");
        }
    }

    // POST: api/chat/conversations/{id}/assistant-message
    [HttpPost("conversations/{id:int}/assistant-message")]
    public async Task<ActionResult<MessageDTO>> TriggerAssistantResponse(int id)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!await _chatService.CanUserAccessConversationAsync(id, userId))
            {
                return Forbid("You don't have access to this conversation");
            }

            var conversation = await _chatService.GetConversationAsync(id);
            if (conversation?.Type != ConversationType.UserToAssistant)
            {
                return BadRequest("This endpoint is only for assistant conversations");
            }

            // Trigger assistant response
            await HandleAssistantResponse(id);

            return Ok(new { message = "Assistant response triggered" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error triggering assistant response: {ex.Message}");
        }
    }

    // POST: api/chat/conversations/{id}/mark-read
    [HttpPost("conversations/{id:int}/mark-read")]
    public async Task<IActionResult> MarkAllMessagesAsRead(int id)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!await _chatService.CanUserAccessConversationAsync(id, userId))
            {
                return Forbid("You don't have access to this conversation");
            }

            await _chatService.MarkAllMessagesAsReadAsync(id, userId);

            // Notify other users that messages were marked as read
            await _hubContext.Clients.Group($"conversation_{id}")
                .SendAsync("MessagesMarkedAsRead", userId);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error marking messages as read: {ex.Message}");
        }
    }

    // GET: api/chat/conversations/{id}/info
    [HttpGet("conversations/{id:int}/info")]
    public async Task<ActionResult<ConversationDTO>> GetConversationInfo(int id)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (!await _chatService.CanUserAccessConversationAsync(id, userId))
            {
                return Forbid("You don't have access to this conversation");
            }

            var conversation = await _chatService.GetConversationWithInfoAsync(id, userId);
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving conversation info: {ex.Message}");
        }
    }

    // GET: api/chat/search
    [HttpGet("search")]
    public async Task<ActionResult<List<MessageDTO>>> SearchMessages([FromQuery] string query, [FromQuery] int? conversationId = null)
    {
        try
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty");
            }

            var messages = await _chatService.SearchMessagesAsync(userId, query, conversationId);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error searching messages: {ex.Message}");
        }
    }

    // Private helper methods
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }

    private async Task HandleAssistantResponse(int conversationId)
    {
        try
        {
            // Show typing indicator
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("AssistantTyping", true);

            // Add delay to simulate thinking
            await Task.Delay(1500);

            // Get assistant response
            var response = await _chatService.GetAssistantResponseAsync(conversationId);

            if (!string.IsNullOrEmpty(response))
            {
                var assistantMessage = await _chatService.SendAssistantMessageAsync(conversationId, response);

                await _hubContext.Clients.Group($"conversation_{conversationId}")
                    .SendAsync("ReceiveMessage", assistantMessage);
            }
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("Error", $"Assistant error: {ex.Message}");
        }
        finally
        {
            // Hide typing indicator
            await _hubContext.Clients.Group($"conversation_{conversationId}")
                .SendAsync("AssistantTyping", false);
        }
    }
}