using MediatR;
using SMART.ERP.Application.DTOs.AccountingConfig;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AccountingConfigSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Queries
{
    /// <summary>Devuelve las 14 cuentas de sistema, con su cuenta del catálogo mapeada (o null).</summary>
    public class GetAccountingMappingsQuery : IRequest<Response<List<AccountingMappingDto>>>
    {
        public class Handler : IRequestHandler<GetAccountingMappingsQuery, Response<List<AccountingMappingDto>>>
        {
            private readonly IReadRepositoryAsync<AccountingMapping> _repository;
            public Handler(IReadRepositoryAsync<AccountingMapping> repository) => _repository = repository;

            public async Task<Response<List<AccountingMappingDto>>> Handle(GetAccountingMappingsQuery request, CancellationToken cancellationToken)
            {
                var existing = await _repository.ListAsync(new AllAccountingMappingsSpecification(), cancellationToken);
                var byKey = existing.ToDictionary(x => x.Key);

                var result = Enum.GetValues<AccountMappingKey>().Select(key =>
                {
                    byKey.TryGetValue(key, out var m);
                    return new AccountingMappingDto
                    {
                        Key = key,
                        KeyName = AccountMappingKeyLabels.Label(key),
                        LedgerAccountId = m?.LedgerAccountId,
                        LedgerAccountCode = m?.LedgerAccount?.Code,
                        LedgerAccountName = m?.LedgerAccount?.Name,
                    };
                }).ToList();

                return new Response<List<AccountingMappingDto>>(result);
            }
        }
    }
}
