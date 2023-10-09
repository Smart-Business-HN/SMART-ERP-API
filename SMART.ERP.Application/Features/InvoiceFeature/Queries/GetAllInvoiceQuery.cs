using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Features.QuotationFeature.Queries;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InvoiceFeature.Queries
{
    public class GetAllInvoiceQuery : IRequest<PagedResponse<List<InvoiceDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllInvoiceQueryHandler : IRequestHandler<GetAllInvoiceQuery, PagedResponse<List<InvoiceDto>>>
    {
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllInvoiceQueryHandler(IRepositoryAsync<Invoice> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<InvoiceDto>>> Handle(GetAllInvoiceQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var quotations = await _repositoryAsync.ListAsync(new QueryInvoiceSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<InvoiceDto>>(quotations);
            return new PagedResponse<List<InvoiceDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
