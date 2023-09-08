using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProspectQuoteProductFeature.Commands.CreateProspectQuoteProductCommand;
using SMART.ERP.Application.Features.ProspectQuoteProductFeature.Commands.DeactivateProspectQuoteProductCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [Authorize]
    public class ProspectQuoteProductController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateProspectQuoteProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Deactivate/{Id}")]
        public async Task<IActionResult> Deactivate(int Id, [FromBody] DeactivateProspectQuoteProductCommand command)
        {
            if (Id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
