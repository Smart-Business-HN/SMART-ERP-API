using Microsoft.AspNetCore.SignalR;

namespace SMART.ERP.Application.Services.SignalRHub
{
    public class ChatHub : Hub
    {
        public async Task JoinSession(string sessionIdentifier)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionIdentifier}");
        }

        public async Task AdminJoinSession(string sessionIdentifier)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin_chat_listeners");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chat_{sessionIdentifier}");
        }

        public async Task AdminSubscribe()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin_chat_listeners");
        }

        public async Task TypingIndicator(string sessionIdentifier, string senderType, bool isTyping)
        {
            await Clients.OthersInGroup($"chat_{sessionIdentifier}")
                .SendAsync("UserTyping", senderType, isTyping);
        }

        public async Task LeaveSession(string sessionIdentifier)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat_{sessionIdentifier}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
