using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.DeleteOpportunityCommentCommand;
using SMART.ERP.Application.Features.OpportunityCommentFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.CreateOpportunityCommentCommand;
using SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.UpdateOpportunityCommentCommand;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OpportunityCommentController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetOpportunityCommentByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllOpportunityCommentsQuery()
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
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Create([FromBody] CreateOpportunityCommentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "SuperAdmin, Manager, SalesAdvisor, Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOpportunityCommentCommand command)
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
            return Ok(await Mediator.Send(new DeleteOpportunityCommentCommand { Id = id }));
        }
    }
}
