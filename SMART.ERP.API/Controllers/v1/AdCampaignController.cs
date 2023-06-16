using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.AdCampaignFeature.Queries;
using SMART.ERP.Application.Features.AdCampaignFeature.Commands.SendAdCampaignCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class AdCampaignController : BaseApiController
    {
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromForm] SendAdCampaignCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllMetaAdCampaignQuery()));
        }
    }
}
