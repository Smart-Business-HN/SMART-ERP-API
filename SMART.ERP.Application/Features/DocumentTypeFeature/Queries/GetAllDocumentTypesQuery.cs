using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DocumentTypeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Queries
{
    public class GetAllDocumentTypesQuery : IRequest<PagedResponse<List<DocumentTypeDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllDocumentTypesQueryHandler : IRequestHandler<GetAllDocumentTypesQuery, PagedResponse<List<DocumentTypeDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<DocumentType> _repositoryAsync;

            public GetAllDocumentTypesQueryHandler(IMapper mapper, IRepositoryAsync<DocumentType> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<DocumentTypeDto>>> Handle(GetAllDocumentTypesQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var documentTypes = await _repositoryAsync.ListAsync(new PaginationDocumentTypeSpecification(request.Parameter, request.PageNumber,
                    request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<DocumentTypeDto>>(documentTypes);
                return new PagedResponse<List<DocumentTypeDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
