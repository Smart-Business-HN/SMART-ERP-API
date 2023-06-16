using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.NotificationFeature.Commands.UpdateNotificationCommand;
using SMART.ERP.Application.Features.NotificationFeature.Queries;
using SMART.ERP.API.Controllers;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class NotificationController : BaseApiController
    {
        [HttpPut("Update/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            return Ok(await Mediator.Send(new UpdateNotificationCommand { Id = notificationId}));
        }

        [HttpGet("GetAllUnread/{userId}")]
        public async Task<IActionResult> GetAllUnread(Guid userId)
        {
            return Ok(await Mediator.Send(new GetAllNotificationsUnreadByUserQuery
            {
                UserId = userId
            }));
        }
    }
}
