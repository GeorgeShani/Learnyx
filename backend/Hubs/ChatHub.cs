using learnyx.Models.DTOs;
using learnyx.Models.Enums;
using System.Security.Claims;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace learnyx.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task JoinConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        var canJoin = await _chatService.CanUserAccessConversationAsync(conversationId, userId);
        
        if (canJoin)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("UserJoined", userId, Context.User?.Identity?.Name);
        }
    }

    public async Task LeaveConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("UserLeft", userId, Context.User?.Identity?.Name);
    }

    public async Task SendMessage(int conversationId, string textContent, List<MessageContentDTO> contents)
    {
        try
        {
            var userId = GetCurrentUserId();
            var message = await _chatService.SendMessageAsync(conversationId, userId, textContent, contents);
            
            // Send to all users in the conversation
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", message);

            // Check if this is an assistant conversation and trigger bot response
            var conversation = await _chatService.GetConversationAsync(conversationId);
            if (conversation?.Type == ConversationType.UserToAssistant)
            {
                _ = Task.Run(async () => await HandleAssistantResponse(conversationId, userId));
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Failed to send message: {ex.Message}");
        }
    }

    public async Task MarkMessageAsRead(int messageId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _chatService.MarkMessageAsReadAsync(messageId, userId);
            
            var message = await _chatService.GetMessageAsync(messageId);
            if (message != null)
            {
                await Clients.Group($"conversation_{message.ConversationId}")
                    .SendAsync("MessageRead", messageId, userId);
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Failed to mark message as read: {ex.Message}");
        }
    }

    public async Task StartTyping(int conversationId)
    {
        var userId = GetCurrentUserId();
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("UserTyping", userId, Context.User?.Identity?.Name);
    }

    public async Task StopTyping(int conversationId)
    {
        var userId = GetCurrentUserId();
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("UserStoppedTyping", userId, Context.User?.Identity?.Name);
    }

    public async Task MarkAllMessagesAsRead(int conversationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _chatService.MarkAllMessagesAsReadAsync(conversationId, userId);
            
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("MessagesMarkedAsRead", userId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"Failed to mark messages as read: {ex.Message}");
        }
    }

    private async Task HandleAssistantResponse(int conversationId, int userId)
    {
        try
        {
            // Show typing indicator
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("AssistantTyping", true);

            // Get assistant response
            var response = await _chatService.GetAssistantResponseAsync(conversationId);
            
            if (!string.IsNullOrEmpty(response))
            {
                var assistantMessage = await _chatService.SendAssistantMessageAsync(conversationId, response);
                
                await Clients.Group($"conversation_{conversationId}")
                    .SendAsync("ReceiveMessage", assistantMessage);
            }
        }
        catch (Exception ex)
        {
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("Error", $"Assistant error: {ex.Message}");
        }
        finally
        {
            // Hide typing indicator
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("AssistantTyping", false);
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
