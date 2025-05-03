using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AccountsReivable;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountsReceivableFeature.Queries
{
    public class GetAllAccountsReceivableQuery : IRequest<PagedResponse<List<AccountsReceivableDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllAccountsReceivableQueryHandler : IRequestHandler<GetAllAccountsReceivableQuery, PagedResponse<List<AccountsReceivableDto>>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllAccountsReceivableQueryHandler(IRepositoryAsync<Customer> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<AccountsReceivableDto>>> Handle(GetAllAccountsReceivableQuery request, CancellationToken cancellationToken)
        {
            var accountsRecivables = new List<AccountsReceivableDto>();
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
            }
            var customers = await _repositoryAsync.ListAsync(new FilterCustomerWithPendingInvoicesSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column), cancellationToken);
            customers.ForEach(customer =>
            {
                var invoices = customer.PendingInvoices;
                accountsRecivables.Add(new AccountsReceivableDto
                {
                    CustomerId = customer.Id,
                    Customer = _mapper.Map<CustomerDto>(customer),
                    TotalAmount = invoices!.Sum(i => i.Outstanding),
                    OverdueAmount = invoices!.Where(i => i.ExpectedPaymentDate.HasValue && DateTime.Now.Date > i.ExpectedPaymentDate.Value.ToDateTime(TimeOnly.MinValue)).Sum(i => i.Outstanding),
                    TotalInvoices = invoices!.Count(i => i.Outstanding > 0),
                    Invoices = _mapper.Map<List<InvoiceDto>>(invoices)
                });
            });


            return new PagedResponse<List<AccountsReceivableDto>>(accountsRecivables, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync(new FilterCustomerWithPendingInvoicesSpecification(null, 0, await _repositoryAsync.CountAsync(cancellationToken), request.Order, request.Column),cancellationToken));
        }
    }
}
