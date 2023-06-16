using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorDashboard
{
    public class AdvisorDashboardOpportunityStepMetricsQuery : IRequest<Response<List<OpportunityStepMetricDto>>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AdvisorDashboardOpportunityStepMetricsQueryHandler : IRequestHandler<AdvisorDashboardOpportunityStepMetricsQuery, Response<List<OpportunityStepMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRepositoryHNAsync<Client> _clientRepositoryAsync;

        public AdvisorDashboardOpportunityStepMetricsQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<OpportunityStep> stepRepositoryAsync, IJwtService jwtService, IMapper mapper, IRepositoryHNAsync<Client> clientRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
            _clientRepositoryAsync = clientRepositoryAsync;
        }

        public async Task<Response<List<OpportunityStepMetricDto>>> Handle(AdvisorDashboardOpportunityStepMetricsQuery request, CancellationToken cancellationToken)
        {
            var guid = _jwtService.GetUidToken();
            var checkUser = await _userRepositoryAsync.GetByIdAsync(guid);
            if (checkUser == null)
            {
                throw new InvalidOperationException("Usuario invalido");
            }
            var response = new List<OpportunityStepMetricDto>();
            //Opportunity Step Metrics
            var userSteps = await _repositoryAsync.ListAsync(new FilterOpportunityByUserSpecification(guid, false));
            if (request.StartDate != null)
            {
                var notClosed = userSteps.FindAll(x => x.CreationDate.Date >= request.StartDate!.Value.Date && x.ClosingDate == null);
                var closed = userSteps.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date >= request.StartDate!.Value.Date);
                userSteps = new HashSet<Opportunity>(notClosed.Concat(closed)).ToList();
            }
            if (request.EndDate != null)
            {
                var notClosedEndDate = userSteps.FindAll(x => x.CreationDate.Date <= request.EndDate!.Value.Date && x.ClosingDate == null);
                var closedEndDate = userSteps.FindAll(x => x.ClosingDate.HasValue && x.ClosingDate.Value.Date <= request.EndDate!.Value.Date);
                userSteps = new HashSet<Opportunity>(notClosedEndDate.Concat(closedEndDate)).ToList();
            }
            var opportunitySteps = await _stepRepositoryAsync.ListAsync();
            var clients = await _clientRepositoryAsync.ListAsync();
            foreach (var step in opportunitySteps)
            {
                var opportunityStepMetric = new OpportunityStepMetricDto();
                opportunityStepMetric.Name = step.Name;
                var opportunitiesInStep = userSteps.FindAll(x => x.OpportunityStepId == step.Id);
                opportunityStepMetric.NumOpportunities = opportunitiesInStep.Count;
                if (opportunitiesInStep.Count > 0)
                {
                    opportunityStepMetric.Opportunities = _mapper.Map<List<OpportunityDto>>(opportunitiesInStep);
                    for (int index = 0; index < opportunityStepMetric.Opportunities.Count; index++)
                    {
                        var client = clients.Find(x => x.Id == opportunityStepMetric.Opportunities[index].Customer!.MasterId);
                        if (client != null)
                        {
                            opportunityStepMetric.Opportunities[index].Customer!.MotorsId = opportunityStepMetric.Opportunities[index].CustomerId;
                            opportunityStepMetric.Opportunities[index].Customer!.FullName = client.FullName;
                            opportunityStepMetric.Opportunities[index].Customer!.PhoneNumber = client.PhoneNumber;
                            opportunityStepMetric.Opportunities[index].Customer!.Email = client.Email;
                        }
                    }
                }
                opportunityStepMetric.Total = 0m;
                foreach (var opportunity in opportunitiesInStep)
                {
                    opportunityStepMetric.Total += (decimal)opportunity.Total;
                }
                response.Add(opportunityStepMetric);
            }
            return new Response<List<OpportunityStepMetricDto>>(response);
        }
    }
}
