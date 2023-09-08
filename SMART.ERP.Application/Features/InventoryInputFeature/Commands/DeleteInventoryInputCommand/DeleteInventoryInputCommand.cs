using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Commands.DeleteInventoryInputCommand
{
    public class DeleteInventoryInputCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteInventoryInputCommandHandler : IRequestHandler<DeleteInventoryInputCommand, Response<string>>
    {
        private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;

        public DeleteInventoryInputCommandHandler(IRepositoryAsync<InventoryInput> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteInventoryInputCommand request, CancellationToken cancellationToken)
        {
            var inventoryInput = await _repositoryAsync.GetByIdAsync(request.Id);
            if (inventoryInput == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(inventoryInput);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Ingreso de inventario eliminado correctamente", "Eliminado correctamente");
        }
    }
}
