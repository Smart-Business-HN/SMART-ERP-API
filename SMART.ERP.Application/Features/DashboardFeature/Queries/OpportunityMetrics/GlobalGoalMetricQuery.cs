using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class GlobalGoalMetricQuery : IRequest<Response<GlobalGoalMetricDto>>
    {
        public int BranchOfficeId { get; set; }
    }

    public class GlobalGoalMetricQueryHandler : IRequestHandler<GlobalGoalMetricQuery, Response<GlobalGoalMetricDto>>
    {
        private readonly IRepositoryAsync<AdvisorGoal> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;

        public GlobalGoalMetricQueryHandler(IRepositoryAsync<AdvisorGoal> repositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
        }

        public async Task<Response<GlobalGoalMetricDto>> Handle(GlobalGoalMetricQuery request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;
            var listGoals = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByMonthSpecification(null, request.BranchOfficeId, currentDate));
            var globalGoal = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(DateTime.Now.Year, null));
            GlobalGoalMetricDto response = new GlobalGoalMetricDto();
            response.Global = 0;
            response.GlobalCurrent = 0;
            response.GlobalPercentage = 0;
            response.Total = 0;
            response.Percentage = 0;
            response.Current = 0;
            foreach (var goal in listGoals)
            {
                response.Total += goal.Goal;
            }
            foreach (var goal in globalGoal)
            {
                response.Global += goal.Goal;
            }

            var monthlyWonOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInMonthYearSpecification(currentDate.Month, currentDate.Year, null, request.BranchOfficeId));
            foreach (var opportunity in monthlyWonOpportunities)
            {
                response.Current += opportunity.Total;
            }
            if (response.Current > 0)
            {
                if (response.Current > response.Total)
                {
                    response.Percentage = 100;
                }
                else
                {
                    if (response.Total > 0)
                    {
                        response.Percentage = Math.Round(response.Current / response.Total * 100, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        response.Percentage = 0;
                    }
                }
            }

            var yearWonOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByBranchSpecification(DateTime.Now.Year, request.BranchOfficeId, true));
            foreach (var opportunity in yearWonOpportunities)
            {
                response.GlobalCurrent += opportunity.Total;
            }
            if (response.GlobalCurrent > 0)
            {
                if (response.GlobalCurrent > response.Global)
                {
                    response.GlobalPercentage = 100;
                }
                else
                {
                    if (response.Global > 0)
                    {
                        response.GlobalPercentage = Math.Round(response.GlobalCurrent / response.Global * 100, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        response.GlobalPercentage = 0;
                    }
                }
            }

            return new Response<GlobalGoalMetricDto>(response);
        }
    }
}
