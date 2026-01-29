using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.CompanyFeature.Commands.CreateCompanyCommand;
using SMART.ERP.Application.Features.CompanyFeature.Commands.UpdateCompanyCommand;
using SMART.ERP.Application.Features.CompanyFeature.Queries;

namespace SMART.ERP.API.Controllers.v1
{

    [ApiVersion("1.0")]
    public class CompanyController : BaseApiController
    {
        [HttpGet("GetDetail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetail()
        {
            return Ok(await Mediator.Send(new GetCompanyQuery()));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, CommunityManager, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "Ocurrio un problema con el id de este registro" });
            return Ok(await Mediator.Send(command));
        }

    }
}
