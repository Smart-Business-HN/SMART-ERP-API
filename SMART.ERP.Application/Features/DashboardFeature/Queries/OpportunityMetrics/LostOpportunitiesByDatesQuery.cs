using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class LostOpportunitiesByDatesQuery : IRequest<Response<List<LostOpportunityMetricDto>>>
    {
        public int Year { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class LostOpportunitiesByDatesQueryHandler : IRequestHandler<LostOpportunitiesByDatesQuery, Response<List<LostOpportunityMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IRepositoryAsync<AdvisorGoal> _goalRepositoryAsync;

        public LostOpportunitiesByDatesQueryHandler(IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IRepositoryAsync<AdvisorGoal> goalRepositoryAsync)
        {
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _goalRepositoryAsync = goalRepositoryAsync;
        }

        public async Task<Response<List<LostOpportunityMetricDto>>> Handle(LostOpportunitiesByDatesQuery request, CancellationToken cancellationToken)
        {
            var yearGoal = await _goalRepositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(request.Year, null));
            var response = new List<LostOpportunityMetricDto>();
            string[] months = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            foreach (string month in months)
            {
                var dto = new LostOpportunityMetricDto();
                dto.Month = month;
                dto.NumOpportunities = 0;
                dto.Total = 0;
                dto.GoalTotal = yearGoal.Where(x => x.InitDate.Month == Array.IndexOf(months, month) + 1).Sum(x => x.Goal);
                response.Add(dto);
            }
            List<Opportunity> opportunities;
            opportunities = await _opportunityRepositoryAsync.ListAsync(new FilterLostOpportunitiesinDatesSpecification(request.Year, null, request.BranchOfficeId));

            foreach (var opportunity in opportunities)
            {
                response[opportunity.ClosingDate!.Value.Month - 1].NumOpportunities += 1;
                response[opportunity.ClosingDate!.Value.Month - 1].Total += opportunity.Total;
            }

            opportunities = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(request.Year, null));
            opportunities = opportunities.FindAll(x => x.OpportunityStep!.Name == "Ganado");
            foreach (var opportunity in opportunities)
            {
                response[opportunity.ClosingDate!.Value.Month - 1].WonTotal += opportunity.Total;
            }

            return new Response<List<LostOpportunityMetricDto>>(response);
        }
    }
}
