using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.LoginEcommerceUserCommand;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    public class UserController : BaseApiController
    {
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateEcommerceUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginEcommerceUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // [HttpPost("RecoveryCode")]
        // [AllowAnonymous]
        // public async Task<IActionResult> RecoveryCode([FromBody] CreateForgotCodeCommand command)
        // {
        //     return Ok(await Mediator.Send(command));
        // }
        //
        // [HttpPut("Logout/{userId}")]
        // [AllowAnonymous]
        // public async Task<IActionResult> DeleteSesion(Guid userId)
        // {
        //     return Ok(await Mediator.Send(new RemoveSessionCommand { UserId = userId }));
        // }
    }
}
