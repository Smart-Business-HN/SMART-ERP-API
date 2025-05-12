using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AccountsPayable;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountsPayableFeature.Queries
{
    public class GetAccountPaymentByProviderIdQuery : IRequest<Response<AccountsPayableDto>>
    {
        public int Id { get; set; }
    }
    public class GetAccountPaymentByProviderIdQueryHandler : IRequestHandler<GetAccountPaymentByProviderIdQuery, Response<AccountsPayableDto>>
    {
        private readonly IRepositoryAsync<Provider> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAccountPaymentByProviderIdQueryHandler(IRepositoryAsync<Provider> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<AccountsPayableDto>> Handle(GetAccountPaymentByProviderIdQuery request, CancellationToken cancellationToken)
        {
            var provider = await _repositoryAsync.FirstOrDefaultAsync(new IncludePendingInvoicesByProviderIdSpecification(request.Id),cancellationToken) ?? throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            var dto = new AccountsPayableDto
            {
                ProviderId = provider.Id,
                Provider = _mapper.Map<ProviderDto>(provider),
                TotalAmount = provider.PurchaseBills!.Sum(i => i.Total) + provider.NonBillableExpenses!.Sum(x=>x.Amount),
                TotalOutstanding = provider.PurchaseBills!.Sum(i => i.Outstanding) + provider.NonBillableExpenses!.Sum(x => x.Outstanding),
                PurchaseBills = _mapper.Map<List<PurchaseBillDto>>(provider.PurchaseBills),
                NonBillableExpenses = _mapper.Map<List<NonBillableExpenseDto>>(provider.NonBillableExpenses)
            };
            return new Response<AccountsPayableDto>(dto, "Consulta exitosa");
        }
    }
}
