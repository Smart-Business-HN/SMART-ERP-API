using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.LoginEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.UpdateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.UpdateEcommerceUserProfileePhotoCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserByIdQuery;

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
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await Mediator.Send(new GetEcommerceUserByIdQuery { Id = id }));
        }
        [HttpPut("UpdateProfilePhoto/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePhoto(Guid id, [FromForm] IFormFile file)
        {
            return Ok(await Mediator.Send(new UpdateEcommerceUserProfilePhotoCommand { Id = id, File = file }));
        }
        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEcommerceUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("El ID del usuario no coincide con el ID de la ruta.");
            }
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
