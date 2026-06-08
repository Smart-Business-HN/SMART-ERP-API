using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryExitSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Commands.UpdateInventoryExitCommand
{
    public class UpdateInventoryExitCommand : IRequest<Response<InventoryExitDto>>
    {
        public int Id { get; set; }
        public DateTime ExitDate { get; set; }
        public InventoryExitReason ExitReason { get; set; }
        public string? CustomReason { get; set; }
        public int WarehouseId { get; set; }
        public int? ProjectId { get; set; }
        public string? Notes { get; set; }
        public string? BeneficiaryName { get; set; }
        public List<CreateInventoryExitItemDto> Items { get; set; } = [];

        public class UpdateInventoryExitCommandHandler : IRequestHandler<UpdateInventoryExitCommand, Response<InventoryExitDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<InventoryExit> _exitRepository;
            private readonly IRepositoryAsync<InventoryExitItem> _itemRepository;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;
            private readonly IReadRepositoryAsync<Project> _projectRepository;

            public UpdateInventoryExitCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<InventoryExit> exitRepository, IRepositoryAsync<InventoryExitItem> itemRepository,
                IReadRepositoryAsync<Warehouse> warehouseRepository, IReadRepositoryAsync<Product> productRepository,
                IReadRepositoryAsync<Project> projectRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _exitRepository = exitRepository;
                _itemRepository = itemRepository;
                _warehouseRepository = warehouseRepository;
                _productRepository = productRepository;
                _projectRepository = projectRepository;
            }

            public async Task<Response<InventoryExitDto>> Handle(UpdateInventoryExitCommand request, CancellationToken cancellationToken)
            {
                var exit = await _exitRepository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una salida de inventario con el Id {request.Id}");

                if (exit.Status != InventoryExitStatus.Draft)
                    throw new ApiException("Solo se pueden editar salidas en estado Borrador.");

                if (request.Items == null || request.Items.Count == 0)
                    throw new ApiException("La salida de inventario debe tener al menos un producto.");

                var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén con el Id {request.WarehouseId}");

                if (warehouse.IsVirtual)
                    throw new ApiException("No se permite registrar salidas de inventario en almacenes virtuales (consignados).");

                foreach (var item in request.Items)
                {
                    _ = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new ApiException($"No existe un producto con el Id {item.ProductId}");
                }

                if (request.ProjectId.HasValue)
                {
                    _ = await _projectRepository.GetByIdAsync(request.ProjectId.Value, cancellationToken)
                        ?? throw new ApiException($"No existe un proyecto con el Id {request.ProjectId.Value}");
                }

                var existingItems = await _itemRepository.ListAsync(new InventoryExitItemsByExitSpecification(exit.Id), cancellationToken);
                if (existingItems.Count > 0)
                {
                    await _itemRepository.DeleteRangeAsync(existingItems, cancellationToken);
                }

                exit.ExitDate = request.ExitDate;
                exit.ExitReason = request.ExitReason;
                exit.CustomReason = request.CustomReason;
                exit.WarehouseId = request.WarehouseId;
                exit.ProjectId = request.ProjectId;
                exit.Notes = request.Notes;
                exit.BeneficiaryName = request.BeneficiaryName;
                exit.ModificationDate = DateTime.Now;
                exit.ModifiedBy = _jwtService.GetSubjectToken();
                await _exitRepository.UpdateAsync(exit, cancellationToken);

                var newItems = request.Items.Select(i => new InventoryExitItem
                {
                    InventoryExitId = exit.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Notes = i.Notes
                }).ToList();
                await _itemRepository.AddRangeAsync(newItems, cancellationToken);
                await _itemRepository.SaveChangesAsync(cancellationToken);

                var full = await _exitRepository.FirstOrDefaultAsync(new GetInventoryExitByIdSpecification(exit.Id), cancellationToken);
                var dto = _mapper.Map<InventoryExitDto>(full);
                return new Response<InventoryExitDto>(dto, "Salida de inventario actualizada correctamente.");
            }
        }
    }
}
