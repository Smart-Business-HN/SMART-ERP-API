using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.TaxFeature.Commands.CreateTaxCommand;
using SMART.ERP.Application.Features.TaxFeature.Commands.DeleteTaxCommand;
using SMART.ERP.Application.Features.TaxFeature.Commands.UpdateTaxCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class TaxController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateTaxCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaxCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteTaxCommand { Id = id }));
        }
    }
}
