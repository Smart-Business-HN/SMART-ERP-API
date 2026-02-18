using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ChatFeature.Commands.AssignAdminToChatCommand;
using SMART.ERP.Application.Features.ChatFeature.Commands.CloseChatSessionCommand;
using SMART.ERP.Application.Features.ChatFeature.Commands.CreateChatSessionCommand;
using SMART.ERP.Application.Features.ChatFeature.Commands.MarkChatMessagesReadCommand;
using SMART.ERP.Application.Features.ChatFeature.Commands.SendChatMessageCommand;
using SMART.ERP.Application.Features.ChatFeature.Queries.GetActiveChatSessionsQuery;
using SMART.ERP.Application.Features.ChatFeature.Queries.GetChatMessagesBySessionQuery;
using SMART.ERP.Application.Features.ChatFeature.Queries.GetChatSessionByIdentifierQuery;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class ChatController : BaseApiController
    {
        [HttpPost("CreateSession")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSession([FromBody] CreateChatSessionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("SendMessage")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetSessionByIdentifier/{sessionIdentifier}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSessionByIdentifier(string sessionIdentifier)
        {
            return Ok(await Mediator.Send(new GetChatSessionByIdentifierQuery { SessionIdentifier = sessionIdentifier }));
        }

        [HttpGet("GetMessages/{chatSessionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMessages(int chatSessionId)
        {
            return Ok(await Mediator.Send(new GetChatMessagesBySessionQuery { ChatSessionId = chatSessionId }));
        }

        [HttpGet("GetActiveSessions")]
        [Authorize]
        public async Task<IActionResult> GetActiveSessions()
        {
            return Ok(await Mediator.Send(new GetActiveChatSessionsQuery()));
        }

        [HttpPut("AssignAdmin")]
        [Authorize]
        public async Task<IActionResult> AssignAdmin([FromBody] AssignAdminToChatCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("CloseSession")]
        [Authorize]
        public async Task<IActionResult> CloseSession([FromBody] CloseChatSessionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("MarkRead")]
        [AllowAnonymous]
        public async Task<IActionResult> MarkRead([FromBody] MarkChatMessagesReadCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
