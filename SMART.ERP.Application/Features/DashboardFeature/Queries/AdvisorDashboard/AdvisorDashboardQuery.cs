using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.OpportunityActivitySpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorDashboard
{
    public class AdvisorDashboardQuery : IRequest<Response<AdvisorDashboardDto>>
    {
        public class AdvisorDashboardQueryHandler : IRequestHandler<AdvisorDashboardQuery, Response<AdvisorDashboardDto>>
        {
            private readonly IRepositoryAsync<User> _repositoryAsync;
            private readonly IRepositoryAsync<AdvisorGoal> _advisorGoalRepositoryAsync;
            private readonly IRepositoryAsync<OpportunityActivity> _activityRepositoryAsync;
            private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
            private readonly IJwtService _jwtService;
            private readonly IMapper _mapper;

            public AdvisorDashboardQueryHandler(IRepositoryAsync<User> repositoryAsync, IRepositoryAsync<AdvisorGoal> advisorGoalRepositoryAsync,
                IRepositoryAsync<OpportunityActivity> activityRepositoryAsync, IRepositoryAsync<Opportunity> opportunityRepositoryAsync,
                IJwtService jwtService, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _advisorGoalRepositoryAsync = advisorGoalRepositoryAsync;
                _activityRepositoryAsync = activityRepositoryAsync;
                _opportunityRepositoryAsync = opportunityRepositoryAsync;
                _jwtService = jwtService;
                _mapper = mapper;
            }

            public async Task<Response<AdvisorDashboardDto>> Handle(AdvisorDashboardQuery request, CancellationToken cancellationToken)
            {
                var guid = _jwtService.GetUidToken();
                var checkUser = await _repositoryAsync.GetByIdAsync(guid);
                if (checkUser == null)
                {
                    throw new InvalidOperationException("Usuario invalido");
                }
                var currentDate = DateTime.Now;
                var response = new AdvisorDashboardDto();
                //Advisor Monthly Goal Calcs
                var monthGoal = await _advisorGoalRepositoryAsync.FirstOrDefaultAsync(new FilterAdvisorGoalByMonthSpecification(guid, null, currentDate));
                var annualGoal = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(currentDate.Year, guid));
                if (monthGoal == null)
                {
                    response.MonthlyGoal = 0;
                    response.MonthlyGoalPercentage = 0;
                }
                else
                {
                    var monthWonOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInMonthYearSpecification(currentDate.Month, currentDate.Year, guid, null));
                    var total = 0m;
                    foreach (var opportunity in monthWonOpportunities)
                    {
                        total += opportunity.Total;
                    }
                    response.MonthlyGoal = monthGoal.Goal;
                    decimal totalCompletion = 0;
                    if (monthGoal.Goal > 0)
                    {
                        totalCompletion = total / monthGoal.Goal * 100;
                    }
                    else
                    {
                        totalCompletion = 0;
                    }

                    if (totalCompletion > 100)
                    {
                        response.MonthlyGoalPercentage = 100;
                    }
                    else
                    {
                        response.MonthlyGoalPercentage = Math.Round(totalCompletion, 2, MidpointRounding.AwayFromZero);
                    }
                }
                if (annualGoal.Count < 0)
                {
                    response.AnnualGoal = 0;
                    response.AnnualGoalPercentage = 0;
                }
                else
                {
                    foreach (var goal in annualGoal)
                    {
                        response.AnnualGoal += goal.Goal;
                    }

                    decimal totalCompletion = 0;
                    var yearSales = await _opportunityRepositoryAsync.ListAsync(new FilterClosedOpportunitiesInYearByUserSpecification(currentDate.Year, guid));
                    yearSales = yearSales.FindAll(x => x.OpportunityStep!.Name == "Ganado");
                    foreach (var opportunity in yearSales)
                    {
                        totalCompletion += opportunity.Total;
                    }

                    if (response.AnnualGoal > 0)
                    {
                        response.AnnualGoalPercentage = Math.Round(totalCompletion / response.AnnualGoal * 100, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        response.AnnualGoalPercentage = 0;
                    }

                    if (response.AnnualGoalPercentage > 100)
                    {
                        response.AnnualGoalPercentage = 100;
                    }
                }
                //Future Opportunity Activities
                var upcomingActivities = await _activityRepositoryAsync.ListAsync(new FilterUpcomingActivitiesByUserSpecification(guid));
                var activitiesDto = _mapper.Map<List<OpportunityActivityDto>>(upcomingActivities);
                response.OpportunityActivities = activitiesDto;

                return new Response<AdvisorDashboardDto>(response);
            }
        }
    }
}
