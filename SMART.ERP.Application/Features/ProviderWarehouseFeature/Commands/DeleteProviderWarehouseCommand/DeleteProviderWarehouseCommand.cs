using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderWarehouseFeature.Commands.DeleteProviderWarehouseCommand
{
    public class DeleteProviderWarehouseCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteProviderWarehouseCommandHandler : IRequestHandler<DeleteProviderWarehouseCommand, Response<int>>
    {
        private readonly IRepositoryAsync<ProviderWarehouse> _repositoryAsync;

        public DeleteProviderWarehouseCommandHandler(IRepositoryAsync<ProviderWarehouse> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteProviderWarehouseCommand request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.GetByIdAsync(request.Id);
            if (record == null)
            {
                throw new ApiException($"Vinculación con ID {request.Id} no encontrada");
            }

            await _repositoryAsync.DeleteAsync(record);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<int>(record.Id, "Vinculación eliminada correctamente");
        }
    }
}
