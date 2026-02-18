using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Commands.UpdateShippingCostConfigurationCommand
{
    public class UpdateShippingCostConfigurationCommand : IRequest<Response<ShippingCostDto>>
    {
        public int Id { get; set; }
        public int? SourceWarehouseId { get; set; }
        public int? SourceProviderId { get; set; }
        public int? SourceCityId { get; set; }
        public int? DestinationCityId { get; set; }
        public int? DestinationDepartmentId { get; set; }
        public int? ProductId { get; set; }
        public decimal MinCost { get; set; }
        public decimal MaxCost { get; set; }
        public decimal DefaultCost { get; set; }
        public string CostType { get; set; } = null!;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
    }

    public class UpdateShippingCostConfigurationCommandHandler : IRequestHandler<UpdateShippingCostConfigurationCommand, Response<ShippingCostDto>>
    {
        private readonly IRepositoryAsync<ShippingCostConfiguration> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateShippingCostConfigurationCommandHandler(
            IRepositoryAsync<ShippingCostConfiguration> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<ShippingCostDto>> Handle(UpdateShippingCostConfigurationCommand request, CancellationToken cancellationToken)
        {
            var record = await _repositoryAsync.GetByIdAsync(request.Id);
            if (record == null)
            {
                throw new ApiException($"Configuración con ID {request.Id} no encontrada");
            }

            record.SourceWarehouseId = request.SourceWarehouseId;
            record.SourceProviderId = request.SourceProviderId;
            record.SourceCityId = request.SourceCityId;
            record.DestinationCityId = request.DestinationCityId;
            record.DestinationDepartmentId = request.DestinationDepartmentId;
            record.ProductId = request.ProductId;
            record.MinCost = request.MinCost;
            record.MaxCost = request.MaxCost;
            record.DefaultCost = request.DefaultCost;
            record.CostType = request.CostType;
            record.Notes = request.Notes;
            record.IsActive = request.IsActive;
            record.Priority = request.Priority;
            record.ModificationDate = DateTime.UtcNow;
            record.ModificatedBy = "SYSTEM"; // TODO: Get from current user context

            await _repositoryAsync.UpdateAsync(record);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ShippingCostDto>(record);
            return new Response<ShippingCostDto>(dto, "Configuración actualizada correctamente");
        }
    }
}
