using Ardalis.Specification;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Commands
{
    /// <summary>Mapea una cuenta de gasto legada a una cuenta del catálogo (vía LedgerAccount.ExpenseAccountId, 1:1).</summary>
    public class SetExpenseAccountMappingCommand : IRequest<Response<int>>
    {
        public int ExpenseAccountId { get; set; }
        public int? LedgerAccountId { get; set; }

        private sealed class LedgerByExpenseSpec : Specification<LedgerAccount>
        {
            public LedgerByExpenseSpec(int expenseAccountId) => Query.Where(x => x.ExpenseAccountId == expenseAccountId);
        }

        public class Handler : IRequestHandler<SetExpenseAccountMappingCommand, Response<int>>
        {
            private readonly IRepositoryAsync<LedgerAccount> _ledgerRepository;
            public Handler(IRepositoryAsync<LedgerAccount> ledgerRepository) => _ledgerRepository = ledgerRepository;

            public async Task<Response<int>> Handle(SetExpenseAccountMappingCommand request, CancellationToken cancellationToken)
            {
                // Limpiar cualquier mapeo previo de esta cuenta de gasto.
                var current = await _ledgerRepository.ListAsync(new LedgerByExpenseSpec(request.ExpenseAccountId), cancellationToken);
                foreach (var prev in current)
                {
                    prev.ExpenseAccountId = null;
                    await _ledgerRepository.UpdateAsync(prev, cancellationToken);
                }

                if (request.LedgerAccountId.HasValue)
                {
                    var account = await _ledgerRepository.GetByIdAsync(request.LedgerAccountId.Value, cancellationToken)
                        ?? throw new ApiException("La cuenta del catálogo no existe.");
                    if (!account.IsPostable || !account.IsActive)
                        throw new ApiException($"La cuenta {account.Code} - {account.Name} no es imputable o está inactiva.");
                    account.ExpenseAccountId = request.ExpenseAccountId;
                    await _ledgerRepository.UpdateAsync(account, cancellationToken);
                }

                return new Response<int>(request.ExpenseAccountId, "Mapeo de cuenta de gasto actualizado.");
            }
        }
    }
}
