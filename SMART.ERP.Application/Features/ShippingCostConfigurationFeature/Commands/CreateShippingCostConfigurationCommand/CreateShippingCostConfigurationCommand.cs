using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ShippingCostConfigurationFeature.Commands.CreateShippingCostConfigurationCommand
{
    public class CreateShippingCostConfigurationCommand : IRequest<Response<ShippingCostDto>>
    {
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
        public bool IsActive { get; set; } = true;
        public int Priority { get; set; } = 1;
    }

    public class CreateShippingCostConfigurationCommandHandler : IRequestHandler<CreateShippingCostConfigurationCommand, Response<ShippingCostDto>>
    {
        private readonly IRepositoryAsync<ShippingCostConfiguration> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateShippingCostConfigurationCommandHandler(
            IRepositoryAsync<ShippingCostConfiguration> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<ShippingCostDto>> Handle(CreateShippingCostConfigurationCommand request, CancellationToken cancellationToken)
        {
            var newRecord = new ShippingCostConfiguration
            {
                SourceWarehouseId = request.SourceWarehouseId,
                SourceProviderId = request.SourceProviderId,
                SourceCityId = request.SourceCityId,
                DestinationCityId = request.DestinationCityId,
                DestinationDepartmentId = request.DestinationDepartmentId,
                ProductId = request.ProductId,
                MinCost = request.MinCost,
                MaxCost = request.MaxCost,
                DefaultCost = request.DefaultCost,
                CostType = request.CostType,
                Notes = request.Notes,
                IsActive = request.IsActive,
                Priority = request.Priority,
                CreationDate = DateTime.UtcNow,
                CreatedBy = "SYSTEM" // TODO: Get from current user context
            };

            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ShippingCostDto>(response);
            return new Response<ShippingCostDto>(dto, "Configuración de costo de envío creada correctamente");
        }
    }
}
