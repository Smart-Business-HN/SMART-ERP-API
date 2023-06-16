using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityCommentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Queries
{
    public class GetAllOpportunityCommentsQuery : IRequest<PagedResponse<List<OpportunityCommentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllOpportunityCommentsQueryHandler : IRequestHandler<GetAllOpportunityCommentsQuery, PagedResponse<List<OpportunityCommentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<OpportunityComment> _repositoryAsync;

            public GetAllOpportunityCommentsQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityComment> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<OpportunityCommentDto>>> Handle(GetAllOpportunityCommentsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var opportunityComments = await _repositoryAsync.ListAsync(
                    new PaginationOpportunityCommentSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<OpportunityCommentDto>>(opportunityComments);
                return new PagedResponse<List<OpportunityCommentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
