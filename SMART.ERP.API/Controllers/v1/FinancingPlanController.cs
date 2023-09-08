using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.FinancingPlanFeature.Commands.DeleteFinancingPlanCommand;
using SMART.ERP.Application.Features.FinancingPlanFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.FinancingPlanFeature.Commands.CreateFinancingPlanCommand;
using SMART.ERP.Application.Features.FinancingPlanFeature.Commands.UpdateFinancingPlanCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class FinancingPlanController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetFinancingPlanByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllFinancingPlansQuery()
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
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateFinancingPlanCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFinancingPlanCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteFinancingPlanCommand { Id = id }));
        }
    }
}
