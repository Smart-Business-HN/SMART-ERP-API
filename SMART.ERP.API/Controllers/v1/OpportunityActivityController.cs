using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.CreateOpportunityActivityCommand;
using SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.DeleteOpportunityActivityCommand;
using SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.UpdateOpportunityActivityCommand;
using SMART.ERP.Application.Features.OpportunityActivityFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OpportunityActivityController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetOpportunityActivityByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllOpportunityActivitiesQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetAllFilter")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> GetAllFilter([FromQuery] FilterActivityDto filter)
        {
            return Ok(await Mediator.Send(new GetAllFilterOpportunityActivity
            {
                UserId = filter.UserId,
                BranchOfficeId = filter.BranchOfficeId,
                InitDate = filter.InitDate,
                EndDate = filter.EndDate
            }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateOpportunityActivityCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOpportunityActivityCommand command)
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
            return Ok(await Mediator.Send(new DeleteOpportunityActivityCommand { Id = id }));
        }
    }
}
