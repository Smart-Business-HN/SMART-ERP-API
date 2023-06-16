using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityDocumentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityDocumentFeature.Queries
{
    public class GetOpportunityDocumentByIdQuery : IRequest<Response<OpportunityDocumentDto>>
    {
        public int Id { get; set; }
    }

    public class GetOpportunityDocumentByIdQueryHandler : IRequestHandler<GetOpportunityDocumentByIdQuery, Response<OpportunityDocumentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityDocument> _repositoryAsync;

        public GetOpportunityDocumentByIdQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityDocument> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<OpportunityDocumentDto>> Handle(GetOpportunityDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var opportunityComment = await _repositoryAsync.FirstOrDefaultAsync(new OpportunityDocumentIncludesSpecification(request.Id));
            if (opportunityComment == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<OpportunityDocumentDto>(opportunityComment);
            return new Response<OpportunityDocumentDto>(dto);
        }
    }
}
