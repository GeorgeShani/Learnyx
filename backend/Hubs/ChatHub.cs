using learnyx.Models.DTOs;
using learnyx.Models.Enums;
using System.Security.Claims;
using learnyx.Models.SignalR;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;

namespace learnyx.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    
    // Static dictionaries to track user presence across all hub instances
    private static readonly ConcurrentDictionary<string, UserConnection> OnlineConnections = new();
    private static readonly ConcurrentDictionary<int, UserPresence> UserPresenceMap = new();

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        var userName = Context.User?.Identity?.Name ?? "Unknown User";
        
        // Add this connection to the tracking
        var userConnection = new UserConnection
        {
            UserId = userId,
            UserName = userName,
            ConnectionId = Context.ConnectionId,
            ConnectedAt = DateTime.UtcNow
        };

        OnlineConnections.TryAdd(Context.ConnectionId, userConnection);

        // Update or create user presence
        UserPresenceMap.AddOrUpdate(userId, 
            new UserPresence 
            { 
                UserId = userId, 
                UserName = userName, 
                IsOnline = true, 
                LastSeen = DateTime.UtcNow,
                ConnectionCount = 1
            },
            (key, existing) => 
            {
                existing.IsOnline = true;
                existing.ConnectionCount++;
                existing.LastSeen = DateTime.UtcNow;
                return existing;
            });

        // Notify all clients that user is online
        await Clients.All.SendAsync("UserOnline", userId, userName);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (OnlineConnections.TryRemove(Context.ConnectionId, out var userConnection))
        {
            var userId = userConnection.UserId;
            var userName = userConnection.UserName;

            // Update user presence
            if (UserPresenceMap.TryGetValue(userId, out var userPresence))
            {
                userPresence.ConnectionCount = Math.Max(0, userPresence.ConnectionCount - 1);
                userPresence.LastSeen = DateTime.UtcNow;

                // If no more connections, mark as offline
                if (userPresence.ConnectionCount == 0)
                {
                    userPresence.IsOnline = false;
                    await Clients.All.SendAsync("UserOffline", userId, userName, userPresence.LastSeen);
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // New method to get all online users
    public async Task GetOnlineUsers()
    {
        var onlineUserIds = UserPresenceMap.Values
            .Where(u => u.IsOnline)
            .Select(u => u.UserId)
            .ToArray();

        await Clients.Caller.SendAsync("OnlineUsers", onlineUserIds);
    }

    // New method to get specific user's last seen
    public async Task GetUserLastSeen(int userId)
    {
        if (UserPresenceMap.TryGetValue(userId, out var presence))
        {
            await Clients.Caller.SendAsync("UserLastSeen", userId, presence.IsOnline ? null : presence.LastSeen);
        }
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

        // Update user activity
        UpdateUserActivity(userId);
    }

    public async Task LeaveConversation(int conversationId)
    {
        var userId = GetCurrentUserId();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("UserLeft", userId, Context.User?.Identity?.Name);
            
        // Update user activity
        UpdateUserActivity(userId);
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

            // Update user activity
            UpdateUserActivity(userId);

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
            
            // Update user activity
            UpdateUserActivity(userId);
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
            
        // Update user activity
        UpdateUserActivity(userId);
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
                
            // Update user activity
            UpdateUserActivity(userId);
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

    // Helper method to update user activity
    private void UpdateUserActivity(int userId)
    {
        if (UserPresenceMap.TryGetValue(userId, out var presence))
        {
            presence.LastSeen = DateTime.UtcNow;
            presence.IsOnline = true;
        }
    }
}