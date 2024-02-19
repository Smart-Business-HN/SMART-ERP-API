using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.CreateMajorExpenseAccountCommand;
using SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.DeleteMajorExpenseAccountCommand;
using SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.UpdateMajorExpenseAccountCommand;
using SMART.ERP.Application.Features.MajorExpenseAccountFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    public class MajorExpenseAccountController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        [OutputCache(PolicyName = "cache_major_expense_account")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllMajorExpenseAccountQuery
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
        public async Task<IActionResult> Create([FromBody] CreateMajorExpenseAccountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteMajorExpenseAccountCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMajorExpenseAccountCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
