using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.ReportFeature.Queries;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class ReportController : BaseApiController
    {
        [HttpGet("TopQuoteProduct")]
        public async Task<IActionResult> TopQuoteProduct([FromQuery] DateParameter filter, [FromQuery] RequestParameter param, [FromQuery] int CategoryId, [FromQuery] int? SubcategoryId)
        {
            return Ok(await Mediator.Send(new TopQuoteProductQuery
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                BranchOfficeId = filter.BranchOfficeId,
                CategoryId = CategoryId,
                SubcategoryId = SubcategoryId,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }

        [HttpGet("TopSoldProduct")]
        public async Task<IActionResult> TopSoldProduct([FromQuery] DateParameter filter, [FromQuery] RequestParameter param)
        {
            return Ok(await Mediator.Send(new TopSoldProductQuery
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                BranchOfficeId = filter.BranchOfficeId,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }

        [HttpGet("AdvisorActivities")]
        public async Task<IActionResult> AdvisorActivities([FromQuery] DateParameter filter, [FromQuery] RequestParameter param)
        {
            return Ok(await Mediator.Send(new FinishedActivitiesQuery
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                BranchOfficeId = filter.BranchOfficeId,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }

        [HttpGet("OpportunityOrigins")]
        public async Task<IActionResult> OpportunityOrigins([FromQuery] DateParameter filter, [FromQuery] RequestParameter param)
        {
            return Ok(await Mediator.Send(new OpportunityOriginReportQuery
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                BranchOfficeId = filter.BranchOfficeId,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }

        [HttpGet("MasterOpportunity")]
        public async Task<IActionResult> MasterOpportunity([FromQuery] DateParameter filter, [FromQuery] RequestParameter param, [FromQuery] Guid? UserId, [FromQuery] int? OpportunityStepId)
        {
            return Ok(await Mediator.Send(new OpportunityMasterReportQuery
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                BranchOfficeId = filter.BranchOfficeId,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All,
                UserId = UserId,
                OpportunityStepId = OpportunityStepId
            }));
        }

        [HttpGet("MasterProspect")]
        public async Task<IActionResult> MasterProspect([FromQuery] Guid? UserId, [FromQuery] int? ProspectStepId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new MasterProspectReportQuery
            {
                UserId = UserId,
                ProspectStepId = ProspectStepId,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All
            }));
        }

        [HttpGet("AdvisorGoalReport")]
        public async Task<IActionResult> AdvisorGoalReport([FromQuery] int Year, [FromQuery] int? BranchOfficeId, [FromQuery] Guid? UserId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new AdvisorGoalReportQuery
            {
                BranchOfficeId = BranchOfficeId,
                Year = Year,
                UserId = UserId,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All
            }));
        }

        [HttpGet("ClientQuoteReport")]
        public async Task<IActionResult> ClientQuoteReport([FromQuery] DateParameter filter, [FromQuery] RequestParameter param)
        {
            return Ok(await Mediator.Send(new ClientQuoteReportQuery
            {
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }

        [HttpGet("ClientByProductReport")]
        public async Task<IActionResult> ClientByProductReport([FromQuery] DateParameter filter, [FromQuery] int ProductId, [FromQuery] RequestParameter param)
        {
            return Ok(await Mediator.Send(new ClientByQuoteProductReportQuery
            {
                ProductId = ProductId,
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                All = param.All
            }));
        }

        [HttpGet("CustomerWalletReport")]
        public async Task<IActionResult> CustomerWalletReport([FromQuery] Guid? UserId, [FromQuery] RequestParameter filter)
        {
            return Ok(await Mediator.Send(new CustomerWalletReportQuery
            {
                Id = UserId,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                All = filter.All
            }));
        }
    }
}
