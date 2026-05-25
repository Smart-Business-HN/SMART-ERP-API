using MediatR;
using SMART.ERP.Application.DTOs.AccountingConfig;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingConfigSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Queries
{
    public class GetBankAccountMappingsQuery : IRequest<Response<List<BankAccountMappingDto>>>
    {
        public class Handler : IRequestHandler<GetBankAccountMappingsQuery, Response<List<BankAccountMappingDto>>>
        {
            private readonly IReadRepositoryAsync<InternalBankAccount> _repository;
            public Handler(IReadRepositoryAsync<InternalBankAccount> repository) => _repository = repository;

            public async Task<Response<List<BankAccountMappingDto>>> Handle(GetBankAccountMappingsQuery request, CancellationToken cancellationToken)
            {
                var accounts = await _repository.ListAsync(new AllInternalBankAccountsWithLedgerSpecification(), cancellationToken);
                var result = accounts.Select(a => new BankAccountMappingDto
                {
                    InternalBankAccountId = a.Id,
                    Name = a.Name,
                    LedgerAccountId = a.LedgerAccountId,
                    LedgerAccountCode = a.LedgerAccount?.Code,
                    LedgerAccountName = a.LedgerAccount?.Name,
                }).ToList();
                return new Response<List<BankAccountMappingDto>>(result);
            }
        }
    }
}
