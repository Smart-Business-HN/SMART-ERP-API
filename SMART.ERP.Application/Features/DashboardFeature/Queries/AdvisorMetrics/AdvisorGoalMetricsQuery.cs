using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class AdvisorGoalMetricsQuery : IRequest<Response<List<AdvisorGoalMetricDto>>>
    {
        public DateTime? Date { get; set; }
        public int BranchOfficeId { get; set; }
    }

    public class AdvisorGoalMetricsQueryHandler : IRequestHandler<AdvisorGoalMetricsQuery, Response<List<AdvisorGoalMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<AdvisorGoal> _advisorGoalRepositoryAsync;

        public AdvisorGoalMetricsQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<AdvisorGoal> advisorGoalRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _advisorGoalRepositoryAsync = advisorGoalRepositoryAsync;
        }

        public async Task<Response<List<AdvisorGoalMetricDto>>> Handle(AdvisorGoalMetricsQuery request, CancellationToken cancellationToken)
        {
            var response = new List<AdvisorGoalMetricDto>();
            var advisorGoals = new List<AdvisorGoal>();
            var saleAdvisors = new List<User>();
            var opportunities = new List<Opportunity>();
            if (request.Date != null)
            {
                advisorGoals = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByMonthSpecification(null, request.BranchOfficeId, (DateTime)request.Date));
            }
            else
            {
                DateTime currentDate = DateTime.Now;
                advisorGoals = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByMonthSpecification(null, request.BranchOfficeId, currentDate));
            }

            foreach (var advisor in advisorGoals)
            {
                var dto = new AdvisorGoalMetricDto();
                dto.Id = advisor.UserId;
                dto.FullName = advisor.User!.FullName;
                dto.SalesGoal = (decimal)advisor.Goal;
                dto.Total = 0;
                if (request.Date == null)
                {
                    DateTime currentDate = DateTime.Now;
                    opportunities = await _repositoryAsync.ListAsync(new FilterClosedOpportunitiesInMonthYearSpecification(currentDate.Month, currentDate.Year, advisor.UserId, null));

                }
                else
                {
                    opportunities = await _repositoryAsync.ListAsync(new FilterClosedOpportunitiesInMonthYearSpecification(request.Date.Value.Month, request.Date.Value.Year, advisor.UserId, null));
                }
                foreach (var opportunity in opportunities)
                {

                    dto.Total += opportunity.Total;

                }
                response.Add(dto);
            }
            return new Response<List<AdvisorGoalMetricDto>>(response);

        }
    }
}
