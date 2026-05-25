using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.LedgerAccountFeature.Commands.CreateLedgerAccountCommand;
using SMART.ERP.Application.Features.LedgerAccountFeature.Commands.DeleteLedgerAccountCommand;
using SMART.ERP.Application.Features.LedgerAccountFeature.Commands.UpdateLedgerAccountCommand;
using SMART.ERP.Application.Features.LedgerAccountFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class LedgerAccountController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Create([FromBody] CreateLedgerAccountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetLedgerAccountByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_ledger_accounts")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllLedgerAccountsQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetTree")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        [OutputCache(PolicyName = "cache_ledger_accounts")]
        public async Task<IActionResult> GetTree()
        {
            return Ok(await Mediator.Send(new GetChartOfAccountsTreeQuery()));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountant")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLedgerAccountCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrió un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteLedgerAccountCommand { Id = id }));
        }
    }
}
