using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.MessageFeature.Commands.CreateMessageCommand;
using SMART.ERP.Application.Features.MessageFeature.Commands.SendMessageCommand;
using SMART.ERP.Application.Features.MessageFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Services.HeaderService;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MessageController : BaseApiController
    {
        private readonly IHeaderService _headerService;

        public MessageController(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetMessageByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllMessagesQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateMessageCommand command)
        {
            if (!_headerService.VerificatedSecretKey())
                return Unauthorized();

            return Ok(await Mediator.Send(command));
        }

        [HttpPost("Send")]
        [Authorize]
        public async Task<IActionResult> Send([FromForm] SendMessageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
