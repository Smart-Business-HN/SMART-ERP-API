using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityCommentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Queries
{
    public class GetOpportunityCommentByIdQuery : IRequest<Response<OpportunityCommentDto>>
    {
        public int Id { get; set; }
    }

    public class GetOpportunityCommentByIdQueryHandler : IRequestHandler<GetOpportunityCommentByIdQuery, Response<OpportunityCommentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityComment> _repositoryAsync;

        public GetOpportunityCommentByIdQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityComment> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<OpportunityCommentDto>> Handle(GetOpportunityCommentByIdQuery request, CancellationToken cancellationToken)
        {
            var opportunityComment = await _repositoryAsync.FirstOrDefaultAsync(new OpportunityCommentIncludesSpecification(request.Id));
            if (opportunityComment == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<OpportunityCommentDto>(opportunityComment);
            return new Response<OpportunityCommentDto>(dto);
        }
    }
}
