using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Commands
{
    public class SetBankAccountMappingCommand : IRequest<Response<int>>
    {
        public int InternalBankAccountId { get; set; }
        public int? LedgerAccountId { get; set; }

        public class Handler : IRequestHandler<SetBankAccountMappingCommand, Response<int>>
        {
            private readonly IRepositoryAsync<InternalBankAccount> _repository;
            private readonly IReadRepositoryAsync<LedgerAccount> _ledgerRepository;
            public Handler(IRepositoryAsync<InternalBankAccount> repository, IReadRepositoryAsync<LedgerAccount> ledgerRepository)
            {
                _repository = repository;
                _ledgerRepository = ledgerRepository;
            }

            public async Task<Response<int>> Handle(SetBankAccountMappingCommand request, CancellationToken cancellationToken)
            {
                var bank = await _repository.GetByIdAsync(request.InternalBankAccountId, cancellationToken)
                    ?? throw new ApiException("La cuenta bancaria no existe.");
                if (request.LedgerAccountId.HasValue)
                {
                    var account = await _ledgerRepository.GetByIdAsync(request.LedgerAccountId.Value, cancellationToken)
                        ?? throw new ApiException("La cuenta del catálogo no existe.");
                    if (!account.IsPostable || !account.IsActive)
                        throw new ApiException($"La cuenta {account.Code} - {account.Name} no es imputable o está inactiva.");
                }
                bank.LedgerAccountId = request.LedgerAccountId;
                await _repository.UpdateAsync(bank, cancellationToken);
                return new Response<int>(bank.Id, "Mapeo de banco/caja actualizado.");
            }
        }
    }
}
