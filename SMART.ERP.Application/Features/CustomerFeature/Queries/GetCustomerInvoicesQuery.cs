using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetCustomerInvoicesQuery : IRequest<PagedResponse<List<CustomerInvoiceLineDto>>>
    {
        public Guid CustomerId { get; set; }
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetCustomerInvoicesQueryHandler : IRequestHandler<GetCustomerInvoicesQuery, PagedResponse<List<CustomerInvoiceLineDto>>>
    {
        private readonly IRepositoryAsync<Invoice> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetCustomerInvoicesQueryHandler(IRepositoryAsync<Invoice> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<CustomerInvoiceLineDto>>> Handle(GetCustomerInvoicesQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber < 0 ? 0 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var invoices = await _repositoryAsync.ListAsync(
                new FilterInvoiceByCustomerIdSpecification(request.CustomerId, request.Parameter, pageNumber, pageSize),
                cancellationToken);

            var totalItems = await _repositoryAsync.CountAsync(
                new CountInvoiceByCustomerIdSpecification(request.CustomerId, request.Parameter),
                cancellationToken);

            var dto = _mapper.Map<List<CustomerInvoiceLineDto>>(invoices);
            return new PagedResponse<List<CustomerInvoiceLineDto>>(dto, pageNumber, pageSize, totalItems);
        }
    }
}
