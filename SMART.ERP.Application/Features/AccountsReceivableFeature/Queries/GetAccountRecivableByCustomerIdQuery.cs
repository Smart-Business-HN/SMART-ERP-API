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
    public class GetAccountRecivableByCustomerIdQuery : IRequest<Response<AccountsReceivableDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetAccountRecivableByCustomerIdQueryHandler : IRequestHandler<GetAccountRecivableByCustomerIdQuery, Response<AccountsReceivableDto>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAccountRecivableByCustomerIdQueryHandler(IRepositoryAsync<Customer> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<AccountsReceivableDto>> Handle(GetAccountRecivableByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _repositoryAsync.FirstOrDefaultAsync(new IncludePendingInvoicesByCustomerIdSpecification(request.Id));
            if (customer == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = new AccountsReceivableDto
            {
                CustomerId = customer.Id,
                Customer = _mapper.Map<CustomerDto>(customer),
                TotalAmount = customer.PendingInvoices!.Sum(i => i.Outstanding),
                OverdueAmount = customer.PendingInvoices!.Where(i => i.ExpectedPaymentDate.HasValue && DateTime.Now.Date > i.ExpectedPaymentDate.Value.ToDateTime(TimeOnly.MinValue)).Sum(i => i.Outstanding),
                TotalInvoices = customer.PendingInvoices!.Count,
                Invoices = _mapper.Map<List<InvoiceDto>>(customer.PendingInvoices)
            };
            return new Response<AccountsReceivableDto>(dto);
        }
    }
}
