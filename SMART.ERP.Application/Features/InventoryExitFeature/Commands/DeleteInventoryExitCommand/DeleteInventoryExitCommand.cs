using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Commands.DeleteInventoryExitCommand
{
    public class DeleteInventoryExitCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }

        public class DeleteInventoryExitCommandHandler : IRequestHandler<DeleteInventoryExitCommand, Response<int>>
        {
            private readonly IRepositoryAsync<InventoryExit> _exitRepository;

            public DeleteInventoryExitCommandHandler(IRepositoryAsync<InventoryExit> exitRepository)
            {
                _exitRepository = exitRepository;
            }

            public async Task<Response<int>> Handle(DeleteInventoryExitCommand request, CancellationToken cancellationToken)
            {
                var exit = await _exitRepository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una salida de inventario con el Id {request.Id}");

                if (exit.Status != InventoryExitStatus.Draft)
                    throw new ApiException("Solo se pueden eliminar salidas en estado Borrador.");

                await _exitRepository.DeleteAsync(exit, cancellationToken);
                await _exitRepository.SaveChangesAsync(cancellationToken);
                return new Response<int>(exit.Id, "Salida de inventario eliminada correctamente.");
            }
        }
    }
}
