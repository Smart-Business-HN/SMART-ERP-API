using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Queries
{
    public class GetOpportunityStepByIdQuery : IRequest<Response<OpportunityStepDto>>
    {
        public int Id { get; set; }
    }

    public class GetOpportunityStepByIdQueryHandler : IRequestHandler<GetOpportunityStepByIdQuery, Response<OpportunityStepDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityStep> _repositoryAsync;

        public GetOpportunityStepByIdQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityStep> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<OpportunityStepDto>> Handle(GetOpportunityStepByIdQuery request, CancellationToken cancellationToken)
        {
            var opportunityStep = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityStep != null)
            {
                var dto = _mapper.Map<OpportunityStepDto>(opportunityStep);
                return new Response<OpportunityStepDto>(dto);
            }
            else
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
        }
    }
}
