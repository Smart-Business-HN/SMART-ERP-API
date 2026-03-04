using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Commands.DeleteShippingCostConfigurationCommand
{
    public class DeleteShippingCostConfigurationCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
    }

    public class DeleteShippingCostConfigurationCommandHandler : IRequestHandler<DeleteShippingCostConfigurationCommand, Response<int>>
    {
        private readonly IRepositoryAsync<ShippingCostConfiguration> _repositoryAsync;

        public DeleteShippingCostConfigurationCommandHandler(IRepositoryAsync<ShippingCostConfiguration> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteShippingCostConfigurationCommand request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.GetByIdAsync(request.Id);
            if (record == null)
            {
                throw new ApiException($"Configuración con ID {request.Id} no encontrada");
            }

            await _repositoryAsync.DeleteAsync(record);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<int>(record.Id, "Configuración eliminada correctamente");
        }
    }
}
