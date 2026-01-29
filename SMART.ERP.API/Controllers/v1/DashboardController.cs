using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMART.ERP.Application.Features.DashboardFeature.Queries;
using SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard;
using SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics;
using SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics;
using SMART.ERP.Application.Features.DashboardFeature.Queries.ProductMetrics;
using SMART.ERP.Application.Features.DashboardFeature.Queries.ProspectMetrics;
using SMART.ERP.Application.Parameters;

namespace SMART.ERP.API.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class DashboardController : BaseApiController
    {
        [HttpGet("OpportunitiesByAdvisor")]
        public async Task<IActionResult> OpportunitiesByAdvisor([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new OpportunityByUserQuery { BranchOfficeId = BranchOfficeId }));
        }

        [HttpGet("GlobalGoalMetric")]
        public async Task<IActionResult> GlobalGoalMetric([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new GlobalGoalMetricQuery { BranchOfficeId = BranchOfficeId }));
        }

        [HttpPost("ProductsByDate")]
        public async Task<IActionResult> ProductsByDate([FromBody] ProductsByDateQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("SalesFromYear")]
        public async Task<IActionResult> SalesFromDates([FromQuery] YearSaleParameter parameter)
        {
            return Ok(await Mediator.Send(new SalesFromYearQuery { Year = parameter.Year, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("SoldProductByCategory")]
        public async Task<IActionResult> SoldProductByCategory([FromQuery] int? CategoryId)
        {
            return Ok(await Mediator.Send(new SoldProductByCategoryMetricQuery
            {
                CategoryId = CategoryId
            }));
        }

        [HttpGet("ProductSoldComparative")]
        public async Task<IActionResult> ProductSoldComparative([FromQuery] int Year, [FromQuery] int? CategoryId)
        {
            return Ok(await Mediator.Send(new ProductSoldComparativeQuery
            {
                Year = Year,
                CategoryId = CategoryId
            }));
        }

        [HttpGet("NumSalesFromYear")]
        public async Task<IActionResult> NumSalesFromYear([FromQuery] YearSaleParameter filter)
        {
            return Ok(await Mediator.Send(new NumSalesFromYearQuery { Year = filter.Year, BranchOfficeId = filter.BranchOfficeId }));
        }

        [HttpGet("LostOpportunitiesByYear")]
        public async Task<IActionResult> LostOpportunitiesByDate([FromQuery] YearSaleParameter parameter)
        {
            return Ok(await Mediator.Send(new LostOpportunitiesByDatesQuery { Year = parameter.Year, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("OpportunityStepMetrics")]
        public async Task<IActionResult> OpportunityStepMetrics([FromQuery] DateParameter parameter)
        {
            return Ok(await Mediator.Send(new OpportunityFunnelStepMetricQuery { StartDate = parameter.StartDate, EndDate = parameter.EndDate, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("OpportunityFunnelStepMetrics")]
        public async Task<IActionResult> OpportunityFunnelStepMetrics([FromQuery] DateParameter parameter)
        {
            return Ok(await Mediator.Send(new OpportunityStepMetricsQuery { StartDate = parameter.StartDate, EndDate = parameter.EndDate, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("DepartmentOpportunityMetrics")]
        public async Task<IActionResult> DepartmentOpportunityMetrics([FromQuery] DateParameter filter)
        {
            return Ok(await Mediator.Send(new DepartmentOpportunityMetrics { StartDate = filter.StartDate, EndDate = filter.EndDate }));
        }

        [HttpGet("OpportunityLossReasonMetrics")]
        public async Task<IActionResult> OpportunityLossReasonMetrics([FromQuery] DateParameter parameter)
        {
            return Ok(await Mediator.Send(new OpportunityLossReasonsMetricsQuery { StartDate = parameter.StartDate, EndDate = parameter.EndDate, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("OpportunityWinReasonMetrics")]
        public async Task<IActionResult> OpportunityWinReasonMetrics([FromQuery] DateParameter parameter)
        {
            return Ok(await Mediator.Send(new OpportunityWinReasonsMetricsQuery { StartDate = parameter.StartDate, EndDate = parameter.EndDate, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("OpportunityOriginMetrics")]
        public async Task<IActionResult> OpportunityWinReasonMetrics([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new TypeOriginMetricQuery { BranchOfficeId = BranchOfficeId }));
        }

        [HttpGet("TopActiveOpportunitiesMetric")]
        public async Task<IActionResult> TopOpportunitiesMetric([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new TopActiveOpportunitiesMetric { BranchOfficeId = BranchOfficeId }));
        }

        [HttpGet("AdvisorPendingActivities")]
        public async Task<IActionResult> AdvisorPendingActivities([FromQuery] int branchId)
        {
            return Ok(await Mediator.Send(new PendingActivitiesByAdvisorQuery { BranchOfficeId = branchId }));
        }

        [HttpGet("AdvisorGoalMetrics")]
        public async Task<IActionResult> AdvisorGoalMetrics([FromQuery] DateBranchParameter parameter)
        {
            return Ok(await Mediator.Send(new AdvisorGoalMetricsQuery { BranchOfficeId = parameter.BranchOfficeId, Date = parameter.Date }));
        }

        [HttpGet("ProductYearSales")]
        public async Task<IActionResult> ProductYearSales([FromQuery] YearSaleParameter parameter)
        {
            return Ok(await Mediator.Send(new ProductYearSalesMetric { Year = parameter.Year, BranchOfficeId = parameter.BranchOfficeId }));
        }

        [HttpGet("CategoryTotalSalesMetrics")]
        public async Task<IActionResult> CategoryTotalSalesMetrics([FromQuery] DateParameter parameter, [FromQuery] int OpportunityStepId)
        {
            return Ok(await Mediator.Send(new CategoryTotalSalesMetricsQuery { StartDate = parameter.StartDate, EndDate = parameter.EndDate, BranchOfficeId = parameter.BranchOfficeId, OpportunityStepId = OpportunityStepId }));
        }

        [HttpGet("CategoryNumProductSold")]
        public async Task<IActionResult> CategoryNumProductSalesMetrics([FromQuery] DateParameter parameter, [FromQuery] int? OpportunityStepId)
        {
            return Ok(await Mediator.Send(new CategoryNumProductSoldQuery { StartDate = parameter.StartDate, EndDate = parameter.EndDate, BranchOfficeId = parameter.BranchOfficeId, OpportunityStepId = OpportunityStepId }));
        }

        [HttpGet("WinAndLoss")]
        public async Task<IActionResult> WinAndLoss([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new WinAndLossMetricQuery
            {
                BranchOfficeId = BranchOfficeId
            }));
        }

        [HttpGet("ReasonWinAndLoss")]
        public async Task<IActionResult> ReasonWinAndLoss([FromQuery] int BranchOfficeId)
        {
            return Ok(await Mediator.Send(new ReasonWinAndLossOpportunityQuery
            {
                BranchOfficeId = BranchOfficeId
            }));
        }

        [HttpGet("ProspectStepMetric")]
        public async Task<IActionResult> ProspectStepMetric([FromQuery] DateParameter filter)
        {
            return Ok(await Mediator.Send(new ProspectStepsMetricsQuery
            {
                BranchOfficeId = filter.BranchOfficeId,
                StartDate = filter.StartDate,
                EndDate = filter.EndDate
            }));
        }

        [HttpGet("ProspectConvertMetric")]
        public async Task<IActionResult> ProspectConvertMetric([FromQuery] DateParameter filter)
        {
            return Ok(await Mediator.Send(new ProspectConvertMetricsQuery { StartDate = filter.StartDate, EndDate = filter.EndDate }));
        }

        [HttpGet("CampaignRoiMetric")]
        public async Task<IActionResult> CampaignRoiMetric([FromQuery] string Id)
        {
            return Ok(await Mediator.Send(new AdCampaignRoiQuery { Id = Id }));
        }

        [HttpGet("CampaignEffectivenessMetric")]
        public async Task<IActionResult> CampaignEffectivenessMetric([FromQuery] string Id)
        {
            return Ok(await Mediator.Send(new AdCampaignEffectivenessQuery { Id = Id }));
        }
        [HttpGet("AccountsReceivableVsAccountsPayable")]
        public async Task<IActionResult> AccountsReceivableVsAccountsPayable()
        {
            return Ok(await Mediator.Send(new AccountsReceivableVsAccountsPayableQuery()));
        }
        [HttpGet("GeneralFinanceInformation")]
        public async Task<IActionResult> GeneralFinance()
        {
            return Ok(await Mediator.Send(new GeneralFinanceQuery()));
        }

        [HttpGet("FinancialKpis")]
        public async Task<IActionResult> GetFinancialKpis()
        {
            return Ok(await Mediator.Send(new GetFinancialKpisQuery()));
        }

        [HttpGet("AccountsReceivableAging")]
        public async Task<IActionResult> GetAccountsReceivableAging()
        {
            return Ok(await Mediator.Send(new GetAccountsReceivableAgingQuery()));
        }

        [HttpGet("AccountsPayableAging")]
        public async Task<IActionResult> GetAccountsPayableAging()
        {
            return Ok(await Mediator.Send(new GetAccountsPayableAgingQuery()));
        }

        [HttpGet("MonthlyCashFlow")]
        public async Task<IActionResult> GetMonthlyCashFlow([FromQuery] int months = 12)
        {
            return Ok(await Mediator.Send(new GetMonthlyCashFlowQuery { Months = months }));
        }

        [HttpGet("SalesTrend")]
        public async Task<IActionResult> GetSalesTrend([FromQuery] int months = 12)
        {
            return Ok(await Mediator.Send(new GetSalesTrendQuery { Months = months }));
        }

        [HttpGet("TopCustomersOutstanding")]
        public async Task<IActionResult> GetTopCustomersOutstanding([FromQuery] int top = 5)
        {
            return Ok(await Mediator.Send(new GetTopCustomersOutstandingQuery { Top = top }));
        }

        [HttpGet("TopSuppliersOutstanding")]
        public async Task<IActionResult> GetTopSuppliersOutstanding([FromQuery] int top = 5)
        {
            return Ok(await Mediator.Send(new GetTopSuppliersOutstandingQuery { Top = top }));
        }

        [HttpGet("SalesPipeline")]
        public async Task<IActionResult> GetSalesPipeline()
        {
            return Ok(await Mediator.Send(new GetSalesPipelineQuery()));
        }
    }
}
