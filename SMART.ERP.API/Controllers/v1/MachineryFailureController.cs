using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.MachineryFailureFeature.Commands.DeleteMachineryFailureCommand;
using SMART.ERP.Application.Features.MachineryFailureFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.MachineryFailureFeature.Commands.CreateMachineryFailureCommand;
using SMART.ERP.Application.Features.MachineryFailureFeature.Commands.UpdateMachineryFailureCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class MachineryFailureController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllMachineryFailuresQuery()
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
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateMachineryFailureCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMachineryFailureCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este regitro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteMachineryFailureCommand { Id = id }));
        }
    }
}
