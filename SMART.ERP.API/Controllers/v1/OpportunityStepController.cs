using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpportunityStepFeature.Queries;
using SMART.ERP.API.Controllers;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class OpportunityStepController : BaseApiController
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int BranchOfficeId, [FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
        {
            return Ok(await Mediator.Send(new GetAllOpportunityStepQuery
            {
                BranchOfficeId = BranchOfficeId,
                StartDate = StartDate,
                EndDate = EndDate
            }));
        }
    }
}
