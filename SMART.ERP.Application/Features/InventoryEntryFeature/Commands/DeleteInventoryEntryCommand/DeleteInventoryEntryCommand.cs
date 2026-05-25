using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Commands.DeleteInventoryEntryCommand
{
    public class DeleteInventoryEntryCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }

        public class DeleteInventoryEntryCommandHandler : IRequestHandler<DeleteInventoryEntryCommand, Response<int>>
        {
            private readonly IRepositoryAsync<InventoryEntry> _entryRepository;

            public DeleteInventoryEntryCommandHandler(IRepositoryAsync<InventoryEntry> entryRepository)
            {
                _entryRepository = entryRepository;
            }

            public async Task<Response<int>> Handle(DeleteInventoryEntryCommand request, CancellationToken cancellationToken)
            {
                var entry = await _entryRepository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");

                if (entry.Status != InventoryEntryStatus.Draft)
                    throw new ApiException("Solo se pueden eliminar entradas en estado Borrador.");

                await _entryRepository.DeleteAsync(entry, cancellationToken);
                await _entryRepository.SaveChangesAsync(cancellationToken);
                return new Response<int>(entry.Id, "Entrada de inventario eliminada correctamente.");
            }
        }
    }
}
