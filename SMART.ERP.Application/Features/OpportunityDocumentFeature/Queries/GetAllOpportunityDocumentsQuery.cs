using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityDocumentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityDocumentFeature.Queries
{
    public class GetAllOpportunityDocumentsQuery : IRequest<PagedResponse<List<OpportunityDocumentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllOpportunityDocumentsQueryHandler : IRequestHandler<GetAllOpportunityDocumentsQuery, PagedResponse<List<OpportunityDocumentDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<OpportunityDocument> _repositoryAsync;

            public GetAllOpportunityDocumentsQueryHandler(IMapper mapper, IRepositoryAsync<OpportunityDocument> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<OpportunityDocumentDto>>> Handle(GetAllOpportunityDocumentsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var opportunityDocuments = await _repositoryAsync.ListAsync(
                    new PaginationOpportunityDocumentSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<OpportunityDocumentDto>>(opportunityDocuments);
                return new PagedResponse<List<OpportunityDocumentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
