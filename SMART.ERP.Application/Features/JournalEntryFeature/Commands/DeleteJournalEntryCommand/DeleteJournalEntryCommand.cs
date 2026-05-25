using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.JournalEntryFeature.Commands.DeleteJournalEntryCommand
{
    /// <summary>Elimina un asiento. Solo permitido mientras está en estado Borrador.</summary>
    public class DeleteJournalEntryCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }

        public class DeleteJournalEntryCommandHandler : IRequestHandler<DeleteJournalEntryCommand, Response<int>>
        {
            private readonly IRepositoryAsync<JournalEntry> _repositoryAsync;
            private readonly IOutputCacheStore _outputCacheStored;

            public DeleteJournalEntryCommandHandler(IRepositoryAsync<JournalEntry> repositoryAsync, IOutputCacheStore outputCacheStored)
            {
                _repositoryAsync = repositoryAsync;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<int>> Handle(DeleteJournalEntryCommand request, CancellationToken cancellationToken)
            {
                var entry = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe un asiento con el Id {request.Id}.");

                if (entry.Status != JournalEntryStatus.Draft)
                    throw new ApiException("Solo se pueden eliminar asientos en estado Borrador. Un asiento contabilizado solo puede reversarse.");

                await _repositoryAsync.DeleteAsync(entry, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_journal_entries", cancellationToken);

                return new Response<int>(entry.Id, "Asiento en borrador eliminado exitosamente.");
            }
        }
    }
}
