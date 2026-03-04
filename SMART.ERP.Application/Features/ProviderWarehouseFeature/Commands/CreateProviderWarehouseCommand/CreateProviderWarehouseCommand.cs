using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.ProviderWarehouse;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProviderWarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderWarehouseFeature.Commands.CreateProviderWarehouseCommand
{
    public class CreateProviderWarehouseCommand : IRequest<Response<ProviderWarehouseDto>>
    {
        public int ProviderId { get; set; }
        public int WarehouseId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateProviderWarehouseCommandHandler : IRequestHandler<CreateProviderWarehouseCommand, Response<ProviderWarehouseDto>>
    {
        private readonly IRepositoryAsync<ProviderWarehouse> _repositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepository;
        private readonly IRepositoryAsync<Warehouse> _warehouseRepository;
        private readonly IMapper _mapper;

        public CreateProviderWarehouseCommandHandler(
            IRepositoryAsync<ProviderWarehouse> repositoryAsync,
            IRepositoryAsync<Provider> providerRepository,
            IRepositoryAsync<Warehouse> warehouseRepository,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _providerRepository = providerRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<Response<ProviderWarehouseDto>> Handle(CreateProviderWarehouseCommand request, CancellationToken cancellationToken)
        {
            // Validate provider exists
            var provider = await _providerRepository.GetByIdAsync(request.ProviderId);
            if (provider == null)
            {
                throw new ApiException($"Proveedor con ID {request.ProviderId} no encontrado");
            }

            // Validate warehouse exists
            var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId);
            if (warehouse == null)
            {
                throw new ApiException($"Almacén con ID {request.WarehouseId} no encontrado");
            }

            // Check if link already exists
            var spec = new FilterProviderWarehouseByProviderAndWarehouseSpec(request.ProviderId, request.WarehouseId);
            var existing = await _repositoryAsync.FirstOrDefaultAsync(spec);
            if (existing != null)
            {
                throw new ApiException($"Ya existe una vinculación entre el proveedor y el almacén");
            }

            var newRecord = new ProviderWarehouse
            {
                ProviderId = request.ProviderId,
                WarehouseId = request.WarehouseId,
                IsActive = request.IsActive,
                CreationDate = DateTime.UtcNow,
                CreatedBy = "SYSTEM" // TODO: Get from current user context
            };

            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ProviderWarehouseDto>(response);
            return new Response<ProviderWarehouseDto>(dto, "Vinculación creada correctamente");
        }
    }
}
