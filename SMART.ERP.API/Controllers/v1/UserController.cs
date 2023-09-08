using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.RoleFeature.Queries;
using SMART.ERP.Application.Features.UserFeature.Commands.DeleteUserCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.RemoveSessionCommand;
using SMART.ERP.Application.Features.UserFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.UserFeature.Commands.AssignUserWalletCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.ChangeUserPasswordCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.CreateForgotCodeCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.CreateUserCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.ForgotPasswordCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.LoginUserCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.UpdateUserCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await Mediator.Send(new GetUserByIdQuery { Id = id }));
        }

        [HttpGet("GetByEmail/{email}")]
        [Authorize]
        public async Task<IActionResult> GetByEmail(string email)
        {
            return Ok(await Mediator.Send(new GetUserByEmailQuery { Email = email }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllUsersQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetAdvisorsBranch")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> GetAdvisorsBranch([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new GetAdvisorsByBranchQuery { BranchId = BranchOfficeId }));
        }

        [HttpGet("GetRoles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await Mediator.Send(new GetAllRolesQuery()));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("RecoveryCode")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryCode([FromBody] CreateForgotCodeCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Logout/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteSesion(Guid userId)
        {
            return Ok(await Mediator.Send(new RemoveSessionCommand { UserId = userId }));
        }

        [HttpPost("AssignCustomer")]
        [Authorize(Roles = "SuperAdmin, Admin, Manager")]
        public async Task<IActionResult> AssignCustomer([FromBody] AssignUserWalletCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("ChangePassword/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangeUserPasswordCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("ForgotPassword/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(Guid id, [FromBody] ForgotPasswordCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await Mediator.Send(new DeleteUserCommand { Id = id }));
        }
    }
}
