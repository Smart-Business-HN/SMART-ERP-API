using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WarehouseFeature.Commands.DeleteWarehouseCommand
{
    public class DeleteWarehouseCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Warehouse> _repositoryAsync;

        public DeleteWarehouseCommandHandler(IRepositoryAsync<Warehouse> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await _repositoryAsync.FirstOrDefaultAsync(
                new WarehoseIncludesSpecification(request.Id), cancellationToken);

            if (warehouse == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun almacen con el id {request.Id}");
            }

            if (warehouse.InventoryDistributions != null &&
                warehouse.InventoryDistributions.Any(x => x.Quantity > 0))
            {
                throw new ApiException("No se puede eliminar un almacen que tiene inventario disponible.");
            }

            await _repositoryAsync.DeleteAsync(warehouse, cancellationToken);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{warehouse.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
