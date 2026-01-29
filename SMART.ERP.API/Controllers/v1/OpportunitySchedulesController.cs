using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpportunitySchedulesFeature.Commands.UpdateOpportunityScheduleCommand;
using SMART.ERP.Application.Features.OpportunitySchedulesFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class OpportunitySchedulesController : BaseApiController
    {
        [HttpPost("UpdateSchedule")]
        public async Task<IActionResult> UpdateSchedule([FromBody] UpdateOpportunityScheduleCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetByUserId()
        {
            return Ok(await Mediator.Send(new GetOpportunityScheduleByUserIdQuery()));
        }
    }
}
