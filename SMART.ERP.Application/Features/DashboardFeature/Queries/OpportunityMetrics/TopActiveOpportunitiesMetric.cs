using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class TopActiveOpportunitiesMetric : IRequest<Response<List<OpportunityDto>>>
    {
        public int BranchOfficeId { get; set; }
    }

    public class TopActiveOpportunitiesMetricHandler : IRequestHandler<TopActiveOpportunitiesMetric, Response<List<OpportunityDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _clientRepositoryAsync;
        private readonly IMapper _mapper;

        public TopActiveOpportunitiesMetricHandler(IRepositoryAsync<Opportunity> repositoryAsync,
            IRepositoryAsync<Customer> clientRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<List<OpportunityDto>>> Handle(TopActiveOpportunitiesMetric request, CancellationToken cancellationToken)
        {
            var topOpp = await _repositoryAsync.ListAsync(new TopActiveOpportunitiesSpecification(request.BranchOfficeId));
            var dto = _mapper.Map<List<OpportunityDto>>(topOpp);
            for (int index = 0; index < dto.Count; index++)
            {
                var client = await _clientRepositoryAsync.GetByIdAsync(dto[index].Customer!.MotorsId);
                if (client != null)
                {
                    dto[index].Customer!.FullName = client.FullName;
                    dto[index].Customer!.PhoneNumber = client.PhoneNumber;
                    dto[index].Customer!.Email = client.Email;
                }
            }
            return new Response<List<OpportunityDto>>(dto);
        }
    }
}
