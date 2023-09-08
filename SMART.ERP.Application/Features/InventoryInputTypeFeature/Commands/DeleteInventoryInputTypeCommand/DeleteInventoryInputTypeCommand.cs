using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Commands.DeleteInventoryInputTypeCommand
{
    public class DeleteInventoryInputTypeCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteInventoryInputTypeCommandHandler : IRequestHandler<DeleteInventoryInputTypeCommand, Response<string>>
    {
        private readonly IRepositoryAsync<InventoryInputType> _repositoryAsync;

        public DeleteInventoryInputTypeCommandHandler(IRepositoryAsync<InventoryInputType> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteInventoryInputTypeCommand request, CancellationToken cancellationToken)
        {
            var inventoryInputType = await _repositoryAsync.GetByIdAsync(request.Id);
            if (inventoryInputType == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(inventoryInputType);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{inventoryInputType.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
