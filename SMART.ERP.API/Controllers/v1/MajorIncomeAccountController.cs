using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.CreateMajorIncomeAccountCommand;
using SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.DeleteMajorIncomeAccountCommand;
using SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.UpdateMajorIncomeAccountCommand;
using SMART.ERP.Application.Features.MajorIncomeAccountFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    public class MajorIncomeAccountController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        [OutputCache(PolicyName = "cache_major_income_account")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllMajorIncomeAccountQuery
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
        public async Task<IActionResult> Create([FromBody] CreateMajorIncomeAccountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteMajorIncomeAccountCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMajorIncomeAccountCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
