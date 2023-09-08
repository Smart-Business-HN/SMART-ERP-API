using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpportunityFeature.Commands.DeleteOpportunityCommand;
using SMART.ERP.Application.Features.OpportunityFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCartCommand;
using SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCommand;
using SMART.ERP.Application.Features.OpportunityFeature.Commands.UpdateOpportunityCommand;
using SMART.ERP.Application.Features.OpportunityFeature.Commands.UpdateOpportunityPositionCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OpportunityController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetOpportunityByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllOpportunitiesQuery()
            {
                Parameter = filter.Parameter,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Order = filter.Order,
                Column = filter.Column,
                All = filter.All
            }));
        }

        [HttpGet("GetAllBranch")]
        [Authorize]
        public async Task<IActionResult> GetAllBranch([FromQuery] int BranchOfficeId, [FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate, [FromQuery] string? Parameter)
        {
            return Ok(await Mediator.Send(new GetAllOpportunitiesByBranchQuery
            {
                BranchOfficeId = BranchOfficeId,
                StartDate = StartDate,
                EndDate = EndDate,
                Parameter = Parameter
            }));
        }

        [HttpPut("UpdatePosition/{id}")]
        [Authorize(Roles = "SalesAdvisor")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] UpdateOpportunityPositionCommand command)
        {
            if (id != command.OpportunityId)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOpportunityCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Ocurrio un error con el id de este registro");
            }
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("Active/{customerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(Guid customerId)
        {
            return Ok(await Mediator.Send(new GetLastOpportunityQuery { CustomerId = customerId }));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor")]
        public async Task<IActionResult> Create([FromBody] CreateOpportunityCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost("CreateFromCart")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateFromCart([FromBody] CreateOpportunityCartCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteOpportunityCommand { Id = id }));
        }
    }
}
