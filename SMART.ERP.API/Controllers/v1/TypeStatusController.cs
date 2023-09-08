using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.TypeStatusFeature.Commands.DeleteTypeStatusCommand;
using SMART.ERP.Application.Features.TypeStatusFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.TypeStatusFeature.Commands.CreateTypeStatusCommand;
using SMART.ERP.Application.Features.TypeStatusFeature.Commands.UpdateTypeStatusCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class TypeStatusController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetTypeStatusById(int id)
        {
            return Ok(await Mediator.Send(new GetTypeStatusByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllTypeStatusesQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetActivityStatus")]
        public async Task<IActionResult> GetActivityStatus()
        {
            return Ok(await Mediator.Send(new GetOpportunityActivityStatusQuery()));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateTypeStatusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTypeStatusCommand command)
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
            return Ok(await Mediator.Send(new DeleteTypeStatusCommand { Id = id }));
        }
    }
}
