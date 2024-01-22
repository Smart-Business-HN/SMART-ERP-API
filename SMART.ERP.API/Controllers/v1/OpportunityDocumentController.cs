using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.DeleteOpportunityDocumentCommand;
using SMART.ERP.Application.Features.OpportunityDocumentFeature.Queries;
using SMART.ERP.Application.Parameters;
using SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.CreateOpportunityDocumentCommand;
using Asp.Versioning;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OpportunityDocumentController : BaseApiController
    {
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetOpportunityDocumentByIdQuery { Id = id }));
        }

        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new GetAllOpportunityDocumentsQuery()
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
        public async Task<IActionResult> Create([FromBody] CreateOpportunityDocumentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteOpportunityDocumentCommand { Id = id }));
        }
    }
}
