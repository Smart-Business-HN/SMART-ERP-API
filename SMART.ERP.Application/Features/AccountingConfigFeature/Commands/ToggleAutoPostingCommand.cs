using MediatR;
using SMART.ERP.Application.DTOs.AccountingConfig;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Commands
{
    public class ToggleAutoPostingCommand : IRequest<Response<AccountingSettingsDto>>
    {
        public bool Enabled { get; set; }

        public class Handler : IRequestHandler<ToggleAutoPostingCommand, Response<AccountingSettingsDto>>
        {
            private readonly IRepositoryAsync<AccountingSettings> _repository;
            private readonly IJwtService _jwtService;
            public Handler(IRepositoryAsync<AccountingSettings> repository, IJwtService jwtService)
            {
                _repository = repository;
                _jwtService = jwtService;
            }

            public async Task<Response<AccountingSettingsDto>> Handle(ToggleAutoPostingCommand request, CancellationToken cancellationToken)
            {
                var settings = (await _repository.ListAsync(cancellationToken)).FirstOrDefault();
                if (settings == null)
                {
                    settings = new AccountingSettings { AutoPostingEnabled = request.Enabled, ModificationDate = DateTime.Now, ModifiedBy = _jwtService.GetSubjectToken() };
                    await _repository.AddAsync(settings, cancellationToken);
                }
                else
                {
                    settings.AutoPostingEnabled = request.Enabled;
                    settings.ModificationDate = DateTime.Now;
                    settings.ModifiedBy = _jwtService.GetSubjectToken();
                    await _repository.UpdateAsync(settings, cancellationToken);
                }
                return new Response<AccountingSettingsDto>(new AccountingSettingsDto { AutoPostingEnabled = settings.AutoPostingEnabled },
                    request.Enabled ? "Posteo automático activado." : "Posteo automático desactivado.");
            }
        }
    }
}
