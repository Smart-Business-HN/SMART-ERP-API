using MediatR;
using SMART.ERP.Application.DTOs.AccountingConfig;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Queries
{
    public class GetAccountingSettingsQuery : IRequest<Response<AccountingSettingsDto>>
    {
        public class Handler : IRequestHandler<GetAccountingSettingsQuery, Response<AccountingSettingsDto>>
        {
            private readonly IReadRepositoryAsync<AccountingSettings> _repository;
            public Handler(IReadRepositoryAsync<AccountingSettings> repository) => _repository = repository;

            public async Task<Response<AccountingSettingsDto>> Handle(GetAccountingSettingsQuery request, CancellationToken cancellationToken)
            {
                var settings = (await _repository.ListAsync(cancellationToken)).FirstOrDefault();
                return new Response<AccountingSettingsDto>(new AccountingSettingsDto { AutoPostingEnabled = settings?.AutoPostingEnabled ?? false });
            }
        }
    }
}
