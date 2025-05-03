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

namespace SMART.ERP.Application.Features.AccountsPayableFeature.Queries;

public class GetAllAccountsPayableQuery : IRequest<PagedResponse<List<AccountsPayableDto>>>
{
    public string? Parameter { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Order { get; set; }
    public string? Column { get; set; }
    public bool All { get; set; }
}

public class
    GetAllAccountsPayableQueryHandler : IRequestHandler<GetAllAccountsPayableQuery,
    PagedResponse<List<AccountsPayableDto>>>
{
    private readonly IRepositoryAsync<Provider> _repositoryAsync;
    private readonly IMapper _mapper;

    public GetAllAccountsPayableQueryHandler(IRepositoryAsync<Provider> repositoryAsync, IMapper mapper)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
    }

    public async Task<PagedResponse<List<AccountsPayableDto>>> Handle(GetAllAccountsPayableQuery request,
        CancellationToken cancellationToken)
    {
        var accountsPayables = new List<AccountsPayableDto>();
        if (request.All)
        {
            request.PageNumber = 0;
            request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
        }

        var providers = await _repositoryAsync.ListAsync(
            new FilterProviderWithPendingPurchaseBillsSpecification(request.Parameter, request.PageNumber,
                request.PageSize, request.Order, request.Column), cancellationToken);
        providers.ForEach(supplier =>
        {
            accountsPayables.Add(new AccountsPayableDto
            {
                ProviderId = supplier.Id,
                Provider = _mapper.Map<ProviderDto>(supplier),
                TotalAmount = supplier.PurchaseBills!.Sum(i => i.Total) +
                              supplier.NonBillableExpenses!.Sum(x => x.Amount),
                TotalOutstanding = supplier.PurchaseBills!.Sum(i => i.Outstanding) +
                                   supplier.NonBillableExpenses!.Sum(x => x.Outstanding),
                PurchaseBills = _mapper.Map<List<PurchaseBillDto>>(supplier.PurchaseBills),
                NonBillableExpenses = _mapper.Map<List<NonBillableExpenseDto>>(supplier.NonBillableExpenses)
            });
        });
        return new PagedResponse<List<AccountsPayableDto>>(accountsPayables, request.PageNumber, request.PageSize,
            request.All
                ? request.PageSize
                : await _repositoryAsync.CountAsync(
                    new FilterProviderWithPendingPurchaseBillsSpecification(request.Parameter, request.PageNumber,
                        request.PageSize, request.Order, request.Column), cancellationToken));
    }
}