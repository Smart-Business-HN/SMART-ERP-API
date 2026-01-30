using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Features.ProjectFeature.Commands.CreateProjectCommand;
using SMART.ERP.Application.Features.ProjectFeature.Commands.DeleteProjectCommand;
using SMART.ERP.Application.Features.ProjectFeature.Commands.AssignProjectCommand;
using SMART.ERP.Application.Features.ProjectFeature.Commands.UpdateProjectCommand;
using SMART.ERP.Application.Features.ProjectFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProjectController : BaseApiController
    {
        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProjectCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountan")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProjectByIdQuery { Id = id }));
        }
        [HttpGet("GetAll")]
        [AllowAnonymous]
        [OutputCache(PolicyName = "cache_project")]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProjectsQuery
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }
        [HttpGet("GetFinancialSummary/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin, Accountan")]
        public async Task<IActionResult> GetFinancialSummary(int id)
        {
            return Ok(await Mediator.Send(new GetProjectFinancialSummaryQuery { Id = id }));
        }
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProjectCommand { Id = id }));
        }
        [HttpPost("AssignRecords")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> AssignRecords([FromBody] AssignProjectCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet("GetUnassignedRecords")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> GetUnassignedRecords([FromQuery] string recordType, [FromQuery] string? parameter, [FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10)
        {
            return Ok(await Mediator.Send(new GetUnassignedRecordsQuery
            {
                RecordType = recordType,
                Parameter = parameter,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));
        }
    }
}
