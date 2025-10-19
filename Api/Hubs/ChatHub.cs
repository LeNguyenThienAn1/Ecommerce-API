// Api/Hubs/ChatHub.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // When a client connects, you might map connectionId to user ID in a store if needed
        public override Task OnConnectedAsync()
        {
            // Optionally add to group by user id to send direct messages
            var userId = Context.UserIdentifier; // ensure you set UserIdentifier in map
            if (!string.IsNullOrEmpty(userId))
            {
                Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }
            return base.OnDisconnectedAsync(exception);
        }

        // Optionally expose sending through hub (you can keep business logic in service instead)
        public async Task SendMessage(string receiverId, string message)
        {
            // server-side: broadcast to receiver group
            await Clients.Group($"user-{receiverId}").SendAsync("ReceiveMessage", new
            {
                SenderId = Context.UserIdentifier,
                ReceiverId = receiverId,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
