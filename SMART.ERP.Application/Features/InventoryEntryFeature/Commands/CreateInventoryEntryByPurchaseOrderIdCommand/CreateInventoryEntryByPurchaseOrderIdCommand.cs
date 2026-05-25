using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.DTOs.ProductToPurchase;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Specifications.PrefixSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Commands.CreateInventoryEntryByPurchaseOrderIdCommand
{
    public class CreateInventoryEntryByPurchaseOrderIdCommand : IRequest<Response<InventoryEntryDto>>
    {
        public int WarehouseId { get; set; }
        public int? PrefixId { get; set; }
        public int PurchaseOrderId { get; set; }
        public List<ProductToBuyDto>? ProductEntries { get; set; }

        public class CreateInventoryEntryByPurchaseOrderIdCommandHandler : IRequestHandler<CreateInventoryEntryByPurchaseOrderIdCommand, Response<InventoryEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<InventoryEntry> _entryRepository;
            private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepository;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;

            public CreateInventoryEntryByPurchaseOrderIdCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<InventoryEntry> entryRepository, IRepositoryAsync<PurchaseOrder> purchaseOrderRepository,
                IReadRepositoryAsync<Warehouse> warehouseRepository, IReadRepositoryAsync<Prefix> prefixRepository,
                IReadRepositoryAsync<Product> productRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _entryRepository = entryRepository;
                _purchaseOrderRepository = purchaseOrderRepository;
                _warehouseRepository = warehouseRepository;
                _prefixRepository = prefixRepository;
                _productRepository = productRepository;
            }

            public async Task<Response<InventoryEntryDto>> Handle(CreateInventoryEntryByPurchaseOrderIdCommand request, CancellationToken cancellationToken)
            {
                if (request.ProductEntries == null || request.ProductEntries.Count == 0)
                    throw new ApiException("La orden no tiene productos para ingresar.");

                _ = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén con el Id {request.WarehouseId}");

                var prefix = request.PrefixId.HasValue
                    ? await _prefixRepository.GetByIdAsync(request.PrefixId.Value, cancellationToken)
                    : await _prefixRepository.FirstOrDefaultAsync(new PrefixByFormatSpecification(InventoryPrefixes.Entry), cancellationToken);
                if (prefix == null)
                    throw new ApiException($"No existe el prefijo de Entrada de Inventario ('{InventoryPrefixes.Entry}'). Configúralo en Prefijos.");

                var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(request.PurchaseOrderId, cancellationToken)
                    ?? throw new ApiException($"No existe una orden de compra con el Id {request.PurchaseOrderId}");

                if (purchaseOrder.InventoryEntryDestinationId != null)
                    throw new ApiException("Esta orden de compra ya tiene una entrada de inventario asociada.");

                foreach (var item in request.ProductEntries)
                {
                    if (!item.ProductId.HasValue)
                        throw new ApiException("Una de las líneas de la orden no tiene producto asociado.");
                    _ = await _productRepository.GetByIdAsync(item.ProductId.Value, cancellationToken)
                        ?? throw new ApiException($"No existe un producto con el Id {item.ProductId}");
                }

                var entry = new InventoryEntry
                {
                    EntryType = InventoryEntryType.Purchase,
                    Status = InventoryEntryStatus.Draft,
                    EntryDate = DateTime.Now,
                    WarehouseId = request.WarehouseId,
                    PrefixId = prefix.Id,
                    ProviderId = purchaseOrder.ProviderId,
                    PurchaseOrderOriginId = purchaseOrder.Id,
                    SupplierReference = purchaseOrder.PurchaseOrderCode,
                    Description = $"Generada desde orden de compra {purchaseOrder.PurchaseOrderCode}",
                    CreationDate = DateTime.Now,
                    CreatedBy = _jwtService.GetSubjectToken(),
                    Items = request.ProductEntries.Select(i => new InventoryEntryItem
                    {
                        ProductId = i.ProductId!.Value,
                        Quantity = i.Quantity,
                        UnitCost = i.UnitPrice,
                        Total = i.Quantity * i.UnitPrice,
                        Notes = null
                    }).ToList()
                };

                var created = await _entryRepository.AddAsync(entry, cancellationToken);
                await _entryRepository.SaveChangesAsync(cancellationToken);

                // Vincular la entrada a la orden y avanzar su estado (paridad con el flujo legacy).
                purchaseOrder.InventoryEntryDestinationId = created.Id;
                purchaseOrder.StatusId = purchaseOrder.PurchaseBillDestinationId != null ? 24 : 23;
                await _purchaseOrderRepository.UpdateAsync(purchaseOrder, cancellationToken);
                await _purchaseOrderRepository.SaveChangesAsync(cancellationToken);

                var full = await _entryRepository.FirstOrDefaultAsync(new GetInventoryEntryByIdSpecification(created.Id), cancellationToken);
                var dto = _mapper.Map<InventoryEntryDto>(full);
                return new Response<InventoryEntryDto>(dto, "Entrada de inventario creada desde la orden de compra.");
            }
        }
    }
}
