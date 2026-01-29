using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.DepartmentFeature.Commands.CreateDepartmentCommand;
using SMART.ERP.Application.Features.DepartmentFeature.Commands.DeleteDepartmentCommand;
using SMART.ERP.Application.Features.DepartmentFeature.Commands.UpdateDepartmentCommand;
using SMART.ERP.Application.Features.DepartmentFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DepartmentController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetDepartmentByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_departments")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {

            return Ok(await Mediator.Send(new GetAllDepartmentsQuery
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
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDepartmentCommand { Id = id }));
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }
    }
}
