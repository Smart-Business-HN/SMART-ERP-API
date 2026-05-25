using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Commands.UpdateInventoryEntryCommand
{
    public class UpdateInventoryEntryCommand : IRequest<Response<InventoryEntryDto>>
    {
        public int Id { get; set; }
        public InventoryEntryType EntryType { get; set; }
        public DateTime EntryDate { get; set; }
        public int WarehouseId { get; set; }
        public int? ProviderId { get; set; }
        public string? SupplierReference { get; set; }
        public string? Description { get; set; }
        public List<CreateInventoryEntryItemDto> Items { get; set; } = [];

        public class UpdateInventoryEntryCommandHandler : IRequestHandler<UpdateInventoryEntryCommand, Response<InventoryEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<InventoryEntry> _entryRepository;
            private readonly IRepositoryAsync<InventoryEntryItem> _itemRepository;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;

            public UpdateInventoryEntryCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<InventoryEntry> entryRepository, IRepositoryAsync<InventoryEntryItem> itemRepository,
                IReadRepositoryAsync<Warehouse> warehouseRepository, IReadRepositoryAsync<Product> productRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _entryRepository = entryRepository;
                _itemRepository = itemRepository;
                _warehouseRepository = warehouseRepository;
                _productRepository = productRepository;
            }

            public async Task<Response<InventoryEntryDto>> Handle(UpdateInventoryEntryCommand request, CancellationToken cancellationToken)
            {
                var entry = await _entryRepository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");

                if (entry.Status != InventoryEntryStatus.Draft)
                    throw new ApiException("Solo se pueden editar entradas en estado Borrador.");

                if (request.Items == null || request.Items.Count == 0)
                    throw new ApiException("La entrada de inventario debe tener al menos un producto.");

                _ = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén con el Id {request.WarehouseId}");

                foreach (var item in request.Items)
                {
                    _ = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new ApiException($"No existe un producto con el Id {item.ProductId}");
                }

                // Reemplazar líneas existentes.
                var existingItems = await _itemRepository.ListAsync(new InventoryEntryItemsByEntrySpecification(entry.Id), cancellationToken);
                if (existingItems.Count > 0)
                {
                    await _itemRepository.DeleteRangeAsync(existingItems, cancellationToken);
                }

                entry.EntryType = request.EntryType;
                entry.EntryDate = request.EntryDate;
                entry.WarehouseId = request.WarehouseId;
                entry.ProviderId = request.ProviderId;
                entry.SupplierReference = request.SupplierReference;
                entry.Description = request.Description;
                entry.ModificationDate = DateTime.Now;
                entry.ModifiedBy = _jwtService.GetSubjectToken();
                await _entryRepository.UpdateAsync(entry, cancellationToken);

                var newItems = request.Items.Select(i => new InventoryEntryItem
                {
                    InventoryEntryId = entry.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost,
                    Total = i.Quantity * (i.UnitCost ?? 0m),
                    Notes = i.Notes
                }).ToList();
                await _itemRepository.AddRangeAsync(newItems, cancellationToken);
                await _itemRepository.SaveChangesAsync(cancellationToken);

                var full = await _entryRepository.FirstOrDefaultAsync(new GetInventoryEntryByIdSpecification(entry.Id), cancellationToken);
                var dto = _mapper.Map<InventoryEntryDto>(full);
                return new Response<InventoryEntryDto>(dto, "Entrada de inventario actualizada correctamente.");
            }
        }
    }
}
