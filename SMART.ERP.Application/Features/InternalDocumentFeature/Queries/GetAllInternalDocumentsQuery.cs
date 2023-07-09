using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InternalDocumentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InternalDocumentFeature.Queries
{
    public class GetAllInternalDocumentsQuery : IRequest<PagedResponse<List<InternalDocumentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllInternalDocumentsQueryHandler : IRequestHandler<GetAllInternalDocumentsQuery, PagedResponse<List<InternalDocumentDto>>> {
        private readonly IRepositoryAsync<InternalDocument> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllInternalDocumentsQueryHandler(IRepositoryAsync<InternalDocument> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<InternalDocumentDto>>> Handle(GetAllInternalDocumentsQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var internalDocuments = await _repositoryAsync.ListAsync(new FilterAndPaginationInternalDocumentSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<InternalDocumentDto>>(internalDocuments);
            return new PagedResponse<List<InternalDocumentDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
                }
    }
}
