using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.AccountingConfigFeature.Commands
{
    public class UpdateAccountingMappingCommand : IRequest<Response<int>>
    {
        public AccountMappingKey Key { get; set; }
        public int? LedgerAccountId { get; set; }

        public class Handler : IRequestHandler<UpdateAccountingMappingCommand, Response<int>>
        {
            private readonly IRepositoryAsync<AccountingMapping> _repository;
            private readonly IReadRepositoryAsync<LedgerAccount> _ledgerRepository;
            private readonly IJwtService _jwtService;
            public Handler(IRepositoryAsync<AccountingMapping> repository, IReadRepositoryAsync<LedgerAccount> ledgerRepository, IJwtService jwtService)
            {
                _repository = repository;
                _ledgerRepository = ledgerRepository;
                _jwtService = jwtService;
            }

            public async Task<Response<int>> Handle(UpdateAccountingMappingCommand request, CancellationToken cancellationToken)
            {
                if (request.LedgerAccountId.HasValue)
                {
                    var account = await _ledgerRepository.GetByIdAsync(request.LedgerAccountId.Value, cancellationToken)
                        ?? throw new ApiException("La cuenta seleccionada no existe.");
                    if (!account.IsPostable || !account.IsActive)
                        throw new ApiException($"La cuenta {account.Code} - {account.Name} no es imputable o está inactiva.");
                }

                var mapping = (await _repository.ListAsync(cancellationToken)).FirstOrDefault(x => x.Key == request.Key);
                if (mapping == null)
                {
                    mapping = new AccountingMapping { Key = request.Key, LedgerAccountId = request.LedgerAccountId, ModificationDate = DateTime.Now, ModifiedBy = _jwtService.GetSubjectToken() };
                    await _repository.AddAsync(mapping, cancellationToken);
                }
                else
                {
                    mapping.LedgerAccountId = request.LedgerAccountId;
                    mapping.ModificationDate = DateTime.Now;
                    mapping.ModifiedBy = _jwtService.GetSubjectToken();
                    await _repository.UpdateAsync(mapping, cancellationToken);
                }
                return new Response<int>(mapping.Id, "Mapeo actualizado.");
            }
        }
    }
}
