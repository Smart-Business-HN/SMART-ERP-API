using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.LoginEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.UpdateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.UpdateEcommerceUserProfileePhotoCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.ChangeEcommerceUserPasswordCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserByIdQuery;
using SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserLogsByUserIdQuery;

namespace SMART.ERP.API.Controllers.v2
{
    public class UpdateProfilePhotoRequest
    {
        public IFormFile File { get; set; } = null!;
    }

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
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> UpdateProfilePhoto(Guid id, [FromForm] UpdateProfilePhotoRequest request)
        {
            return Ok(await Mediator.Send(new UpdateEcommerceUserProfilePhotoCommand { Id = id, File = request.File }));
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

        [HttpPut("ChangePassword/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangeEcommerceUserPasswordCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("El ID del usuario no coincide con el ID de la ruta.");
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetLogs/{ecommerceUserId}")]
        [Authorize]
        public async Task<IActionResult> GetLogs(Guid ecommerceUserId, [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10, [FromQuery] int? actionType = null)
        {
            return Ok(await Mediator.Send(new GetEcommerceUserLogsByUserIdQuery
            {
                EcommerceUserId = ecommerceUserId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                ActionType = actionType
            }));
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
