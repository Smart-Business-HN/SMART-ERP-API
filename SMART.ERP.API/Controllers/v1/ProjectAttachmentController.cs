using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.CreateProjectAttachmentCommand;
using SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.DeleteProjectAttachmentCommand;
using SMART.ERP.Application.Features.ProjectAttachmentFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProjectAttachmentController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProjectAttachmentByIdQuery { Id = id }));
        }

        [HttpGet("GetAllByProjectId/{projectId}")]
        [Authorize]
        public async Task<IActionResult> GetAllByProjectId(int projectId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllProjectAttachmentsByProjectIdQuery
            {
                ProjectId = projectId,
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProjectAttachmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProjectAttachmentCommand { Id = id }));
        }
    }
}
