using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityActivitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Queries
{
    public class GetOpportunityActivityByIdQuery : IRequest<Response<OpportunityActivityDto>>
    {
        public int Id { get; set; }
    }

    public class GetOpportunityActivityByIdQueryHandler : IRequestHandler<GetOpportunityActivityByIdQuery, Response<OpportunityActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;

        public GetOpportunityActivityByIdQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityActivity> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<OpportunityActivityDto>> Handle(GetOpportunityActivityByIdQuery request, CancellationToken cancellationToken)
        {
            var opportunityActivity = await _repositoryAsync.FirstOrDefaultAsync(new OpportunityActivityIncludesSpecification(request.Id));
            if (opportunityActivity == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<OpportunityActivityDto>(opportunityActivity);
            return new Response<OpportunityActivityDto>(dto);
        }
    }
}
