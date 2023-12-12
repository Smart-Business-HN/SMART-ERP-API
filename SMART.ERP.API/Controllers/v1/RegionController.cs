using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.RegionFeature.Commands.DeleteRegionCommand;
using SMART.ERP.Application.Features.RegionFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.RegionFeature.Commands.AssignRegionDepartmentCommand;
using SMART.ERP.Application.Features.RegionFeature.Commands.CreateRegionCommand;
using SMART.ERP.Application.Features.RegionFeature.Commands.RemoveRegionDepartmentCommand;
using SMART.ERP.Application.Features.RegionFeature.Commands.UpdateRegionCommand;
using Asp.Versioning;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class RegionController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateRegionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRegionCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("AssignDepartments")]
        public async Task<IActionResult> AssignDepartments([FromBody] AssignRegionDepartmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteRegionCommand { Id = id }));
        }

        [HttpPut("RemoveAssignment")]
        public async Task<IActionResult> RemoveAssignment([FromBody] RemoveRegionDepartmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllRegionsQuery
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Parameter = filter.Parameter,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
    }
}
