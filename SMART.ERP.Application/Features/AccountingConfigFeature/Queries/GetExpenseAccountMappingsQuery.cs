using MediatR;
using SMART.ERP.Application.DTOs.AccountingConfig;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingConfigSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Queries
{
    /// <summary>Lista las cuentas de gasto legadas y la cuenta del catálogo a la que están mapeadas.</summary>
    public class GetExpenseAccountMappingsQuery : IRequest<Response<List<ExpenseAccountMappingDto>>>
    {
        public class Handler : IRequestHandler<GetExpenseAccountMappingsQuery, Response<List<ExpenseAccountMappingDto>>>
        {
            private readonly IReadRepositoryAsync<ExpenseAccount> _expenseRepository;
            private readonly IReadRepositoryAsync<LedgerAccount> _ledgerRepository;
            public Handler(IReadRepositoryAsync<ExpenseAccount> expenseRepository, IReadRepositoryAsync<LedgerAccount> ledgerRepository)
            {
                _expenseRepository = expenseRepository;
                _ledgerRepository = ledgerRepository;
            }

            public async Task<Response<List<ExpenseAccountMappingDto>>> Handle(GetExpenseAccountMappingsQuery request, CancellationToken cancellationToken)
            {
                var expenses = await _expenseRepository.ListAsync(cancellationToken);
                var mapped = await _ledgerRepository.ListAsync(new LedgerAccountsMappedToExpenseSpecification(), cancellationToken);
                var byExpense = mapped.Where(l => l.ExpenseAccountId.HasValue).ToDictionary(l => l.ExpenseAccountId!.Value);

                var result = expenses.Select(e =>
                {
                    byExpense.TryGetValue(e.Id, out var ledger);
                    return new ExpenseAccountMappingDto
                    {
                        ExpenseAccountId = e.Id,
                        Name = e.Name,
                        LedgerAccountId = ledger?.Id,
                        LedgerAccountCode = ledger?.Code,
                        LedgerAccountName = ledger?.Name,
                    };
                }).OrderBy(x => x.Name).ToList();
                return new Response<List<ExpenseAccountMappingDto>>(result);
            }
        }
    }
}
