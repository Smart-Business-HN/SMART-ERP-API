using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdvisorMetrics
{
    public class OpportunityByUserQuery : IRequest<Response<List<OpportunityAdvisorDto>>>
    {
        public int BranchOfficeId { get; set; }
    }

    public class OpportunityByUserQueryHandler : IRequestHandler<OpportunityByUserQuery, Response<List<OpportunityAdvisorDto>>>
    {
        private readonly IRepositoryAsync<User> _repositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IMapper _mapper;

        public OpportunityByUserQueryHandler(IRepositoryAsync<User> repositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<OpportunityAdvisorDto>>> Handle(OpportunityByUserQuery request, CancellationToken cancellationToken)
        {

            var response = new List<OpportunityAdvisorDto>();
            var salesAdvisors = await _repositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", request.BranchOfficeId), cancellationToken);
            foreach (var advisor in salesAdvisors)
            {
                var opportunities = await _opportunityRepositoryAsync.CountAsync(new FilterOpportunityByUserSpecification(advisor.Id, true));
                var dto = _mapper.Map<OpportunityAdvisorDto>(advisor);
                dto.NumOpportunities = opportunities;
                dto.BranchOffice = advisor.BranchOffice!.Name;
                response.Add(dto);
            }
            return new Response<List<OpportunityAdvisorDto>>(response);
        }
    }
}
