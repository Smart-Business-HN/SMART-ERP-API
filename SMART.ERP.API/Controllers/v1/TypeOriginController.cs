using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.TypeOriginFeature.Commands.DeleteTypeOriginCommand;
using SMART.ERP.Application.Features.TypeOriginFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.API.Controllers;
using SMART.ERP.Application.Features.TypeOriginFeature.Commands.CreateTypeOriginCommand;
using SMART.ERP.Application.Features.TypeOriginFeature.Commands.UpdateTypeOriginCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class TypeOriginController : BaseApiController
    {
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllTypeOriginQuery
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
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, CommunityManager")]
        public async Task<IActionResult> Create([FromBody] CreateTypeOriginCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, CommunityManager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTypeOriginCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteTypeOriginCommand { Id = id }));
        }
    }
}
