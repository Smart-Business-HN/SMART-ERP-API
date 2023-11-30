using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.StatusFeature.Commands.DeleteStatusCommand;
using SMART.ERP.Application.Features.StatusFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.StatusFeature.Commands.CreateStatusCommand;
using SMART.ERP.Application.Features.StatusFeature.Commands.UpdateStatusCommand;
using Microsoft.AspNetCore.OutputCaching;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class StatusController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetStatusByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        [OutputCache(PolicyName = "cache_statuses")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllStatusesQuery
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
        public async Task<IActionResult> Create([FromBody] CreateStatusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStatusCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteStatusCommand { Id = id }));
        }
    }
}
