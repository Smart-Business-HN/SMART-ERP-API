using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.CreateAssociatedCompanyCommand;
using SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.DeleteAssociatedCompanyCommand;
using SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.UpdateAssociatedCompanyCommand;
using SMART.ERP.Application.Features.AssociatedCompanyFeature.Queries;

namespace SMART.ERP.API.Controllers.v2
{
    [ApiVersion("2.0")]
    [Authorize]
    public class AssociatedCompanyController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateAssociatedCompanyCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll/{ecommerceUserId}")]
        public async Task<IActionResult> GetAll(Guid ecommerceUserId, [FromQuery] string? parameter, [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10, [FromQuery] string? order = "asc", [FromQuery] string? column = "Name", [FromQuery] bool all = false)
        {
            return Ok(await Mediator.Send(new GetAllAssociatedCompanyQuery
            {
                EcommerceUserId = ecommerceUserId,
                Parameter = parameter,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Order = order,
                Column = column,
                All = all
            }));
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetAssociatedCompanyByIdQuery { Id = id }));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAssociatedCompanyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("El ID no coincide con el ID de la ruta.");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteAssociatedCompanyCommand { Id = id }));
        }
    }
}
