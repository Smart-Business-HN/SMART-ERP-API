using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.DeleteDeliveryDirectionCommand;
using SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.CreateDeliveryDirectionCommand;
using SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.UpdateDeliveryDirectionCommand;
using Asp.Versioning;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class DeliveryDirectionController : BaseApiController
    {
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDeliveryDirectionCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryDirectionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDeliveryDirectionCommand { Id = id }));
        }
    }
}
