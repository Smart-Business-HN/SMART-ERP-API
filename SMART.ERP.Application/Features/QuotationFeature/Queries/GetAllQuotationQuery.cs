using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.QuotationFeature.Queries
{
    public class GetAllQuotationQuery : IRequest<PagedResponse<List<QuotationDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllQuotationQueryHandler : IRequestHandler<GetAllQuotationQuery, PagedResponse<List<QuotationDto>>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllQuotationQueryHandler(IRepositoryAsync<Quotation> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<QuotationDto>>> Handle (GetAllQuotationQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var quotations = await _repositoryAsync.ListAsync(new QueryQuotationSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<QuotationDto>>(quotations);
            return new PagedResponse<List<QuotationDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
