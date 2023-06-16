using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class TypeOriginMetricQuery : IRequest<Response<List<OpportunityReasonsMetricsDto>>>
    {
        public int BranchOfficeId { get; set; }
    }

    public class TypeOriginMetricQueryHandler : IRequestHandler<TypeOriginMetricQuery, Response<List<OpportunityReasonsMetricsDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;

        public TypeOriginMetricQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<TypeOrigin> originRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
        }

        public async Task<Response<List<OpportunityReasonsMetricsDto>>> Handle(TypeOriginMetricQuery request, CancellationToken cancellationToken)
        {
            var origins = await _originRepositoryAsync.ListAsync();
            var response = new List<OpportunityReasonsMetricsDto>();
            foreach (var origin in origins)
            {
                var numOrigins = await _repositoryAsync.ListAsync(new FilterOpportunityByTypeOriginSpecification(origin.Id, request.BranchOfficeId));
                if (numOrigins.Count > 0)
                {
                    var dto = new OpportunityReasonsMetricsDto();
                    dto.Name = origin.Name;
                    dto.Total = numOrigins.Sum(a => a.Total);
                    dto.Quantity = numOrigins.Count;
                    response.Add(dto);
                }
            }
            return new Response<List<OpportunityReasonsMetricsDto>>(response);
        }
    }
}
