using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.CreateInternalBankAccountCommand;
using SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.DeleteInternalBankAccountCommand;
using SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.UpdateInternalBankAccountCommand;
using SMART.ERP.Application.Features.InternalBankAccountFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InternalBankAccountController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetInternalBankAccountById { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllInternalBankAccountsQuery
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
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateInternalBankAccountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteInternalBankAccountCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInternalBankAccountCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
