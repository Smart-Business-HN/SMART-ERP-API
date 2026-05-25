using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.JournalEntryLineSpecification;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Commands.DeleteLedgerAccountCommand
{
    public class DeleteLedgerAccountCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }

        public class DeleteLedgerAccountCommandHandler : IRequestHandler<DeleteLedgerAccountCommand, Response<int>>
        {
            private readonly IRepositoryAsync<LedgerAccount> _repositoryAsync;
            private readonly IRepositoryAsync<JournalEntryLine> _lineRepositoryAsync;
            private readonly IOutputCacheStore _outputCacheStored;

            public DeleteLedgerAccountCommandHandler(IRepositoryAsync<LedgerAccount> repositoryAsync,
                IRepositoryAsync<JournalEntryLine> lineRepositoryAsync, IOutputCacheStore outputCacheStored)
            {
                _repositoryAsync = repositoryAsync;
                _lineRepositoryAsync = lineRepositoryAsync;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<int>> Handle(DeleteLedgerAccountCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una cuenta con el Id {request.Id}.");

                if (entity.IsSystem)
                    throw new ApiException("No se puede eliminar una cuenta del sistema.");

                var children = await _repositoryAsync.CountAsync(new FilterLedgerAccountChildrenSpecification(entity.Id), cancellationToken);
                if (children > 0)
                    throw new ApiException("No se puede eliminar una cuenta que tiene subcuentas.");

                var usedInEntries = await _lineRepositoryAsync.CountAsync(new FilterJournalEntryLineByAccountSpecification(entity.Id), cancellationToken);
                if (usedInEntries > 0)
                    throw new ApiException("No se puede eliminar una cuenta que ya tiene movimientos contables.");

                await _repositoryAsync.DeleteAsync(entity, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_ledger_accounts", cancellationToken);

                return new Response<int>(entity.Id, $"Cuenta {entity.Code} eliminada exitosamente.");
            }
        }
    }
}
