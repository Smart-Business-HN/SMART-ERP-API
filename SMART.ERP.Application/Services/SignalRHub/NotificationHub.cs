using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SMART.ERP.Application.Services.SignalRHub
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
