using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorDashboard;

namespace SMART.ERP.API.Controllers.v1
{
    [Authorize]
    [ApiVersion("1.0")]
    public class AdvisorDashboardController : BaseApiController
    {
        [HttpGet("OpportunityStepMetrics")]
        public async Task<IActionResult> OpportunityStepMetrics([FromQuery] DateTime? StartDate, [FromQuery] DateTime? EndDate)
        {
            return Ok(await Mediator.Send(new AdvisorDashboardOpportunityStepMetricsQuery
            {
                StartDate = StartDate,
                EndDate = EndDate
            }));
        }

        [HttpGet("GetInfo")]
        public async Task<IActionResult> GetInfo()
        {
            return Ok(await Mediator.Send(new AdvisorDashboardQuery()));
        }

        [HttpGet("YearSaleMetric")]
        public async Task<IActionResult> YearSaleMetric([FromQuery] int Year)
        {
            return Ok(await Mediator.Send(new AdvisorDashboardYearSaleMetricsQuery { Year = Year }));
        }
    }
}
