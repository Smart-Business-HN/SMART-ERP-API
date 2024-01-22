using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.AdvisorDepartmentFeature.Commands.DeleteAdvisorDepartmentCommand;
using SMART.ERP.Application.Features.AdvisorDepartmentFeature.Queries;
using SMART.ERP.Application.Features.AdvisorDepartmentFeature.Commands.CreateAdvisorDepartmentCommand;
using Asp.Versioning;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "SuperAdmin, Manager, Admin")]
    public class AdvisorDepartmentController : BaseApiController
    {
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateAdvisorDepartmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteAdvisorDepartmentCommand { Id = id }));

        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllAdvisorDepartmentQuery()));
        }
    }
}
