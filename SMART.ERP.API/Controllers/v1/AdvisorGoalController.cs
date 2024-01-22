using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.AdvisorGoalFeature.Queries;
using SMART.ERP.Application.Features.AdvisorGoalFeature.Commands.UpdateAdvisorGoalCommand;
using Asp.Versioning;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "SuperAdmin, Manager, Admin")]
    public class AdvisorGoalController : BaseApiController
    {

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAdvisorGoalCommand command)
        {
            if (id != command.UserId)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] DateTime date)
        {
            return Ok(await Mediator.Send(new GetAllAdvisorGoalQuery { Date = date }));
        }

        [HttpGet("CheckById/{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            return Ok(await Mediator.Send(new CheckAdvisorGoalQuery { UserId = userId }));
        }
    }
}
