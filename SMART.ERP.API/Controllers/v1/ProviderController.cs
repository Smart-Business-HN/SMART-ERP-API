using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProviderFeature.Commands.DeleteProviderCommand;
using SMART.ERP.Application.Features.ProviderFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.ProviderFeature.Commands.CreateProviderCommand;
using SMART.ERP.Application.Features.ProviderFeature.Commands.UpdateProviderCommand;
using Microsoft.AspNetCore.OutputCaching;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProviderController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProviderByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        [OutputCache(PolicyName = "cache_providers")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProvidersQuery()
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
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Create([FromBody] CreateProviderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProviderCommand command)
        {
            if (id != command.Id)
                return BadRequest(new
                {
                    message = "Ocurrio un problema con el id de este regitro"
                });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProviderCommand { Id = id }));
        }
    }
}
