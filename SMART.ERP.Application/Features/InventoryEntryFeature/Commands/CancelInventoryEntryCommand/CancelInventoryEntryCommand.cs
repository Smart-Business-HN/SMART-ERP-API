using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Commands.CancelInventoryEntryCommand
{
    public class CancelInventoryEntryCommand : IRequest<Response<InventoryEntryDto>>
    {
        public int Id { get; set; }
        public string? CancellationReason { get; set; }

        public class CancelInventoryEntryCommandHandler : IRequestHandler<CancelInventoryEntryCommand, Response<InventoryEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<InventoryEntry> _entryRepository;
            private readonly IRepositoryAsync<InventoryEntryItem> _itemRepository;
            private readonly IRepositoryAsync<Product> _productRepository;
            private readonly IReadRepositoryAsync<InventoryDistribution> _distributionRepository;
            private readonly IAccountingPostingService _accountingPostingService;

            public CancelInventoryEntryCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<InventoryEntry> entryRepository, IRepositoryAsync<InventoryEntryItem> itemRepository,
                IRepositoryAsync<Product> productRepository, IReadRepositoryAsync<InventoryDistribution> distributionRepository,
                IAccountingPostingService accountingPostingService)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _entryRepository = entryRepository;
                _itemRepository = itemRepository;
                _productRepository = productRepository;
                _distributionRepository = distributionRepository;
                _accountingPostingService = accountingPostingService;
            }

            public async Task<Response<InventoryEntryDto>> Handle(CancelInventoryEntryCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var entry = await _entryRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");

                    if (entry.Status != InventoryEntryStatus.Confirmed)
                        throw new ApiException("Solo se pueden cancelar entradas confirmadas.");

                    var items = await _itemRepository.ListAsync(new InventoryEntryItemsByEntrySpecification(entry.Id), ct);
                    var isAbsolute = entry.EntryType is InventoryEntryType.Adjustment or InventoryEntryType.OpeningStock;

                    // Validar que el stock alcanza para revertir compras/entradas normales.
                    if (!isAbsolute)
                    {
                        foreach (var item in items)
                        {
                            var distribution = await _distributionRepository.FirstOrDefaultAsync(
                                new FilterInventoryDistributionByProductIdAndWarehouseId(item.ProductId, entry.WarehouseId), ct);
                            var available = distribution?.Quantity ?? 0m;
                            if (available < item.Quantity)
                                throw new ApiException($"Stock insuficiente para revertir el producto {item.ProductId}. Disponible: {available}, requerido: {item.Quantity}.");
                        }
                    }

                    // Restaurar el costo previo del producto (deduplicado por producto para evitar conflictos de rastreo).
                    var costByProduct = items
                        .Where(i => i.PreviousCostPrice.HasValue)
                        .GroupBy(i => i.ProductId)
                        .ToDictionary(g => g.Key, g => g.First().PreviousCostPrice!.Value);
                    foreach (var kv in costByProduct)
                    {
                        var product = await _productRepository.FirstOrDefaultAsync(new ProductByIdIgnoreFiltersSpecification(kv.Key), ct);
                        if (product != null)
                        {
                            product.CostPrice = kv.Value;
                            await _productRepository.UpdateAsync(product, ct);
                        }
                    }

                    // Revertir stock y Kardex.
                    await _movementService.RecordCancellationForDocumentAsync(
                        "InventoryEntry", entry.Id, DateTime.Now, KardexMovementType.EntryCancellation, userId, userName, ct);

                    foreach (var productId in items.Select(i => i.ProductId).Distinct())
                    {
                        await _movementService.SyncProductStockAsync(productId, ct);
                    }

                    entry.Status = InventoryEntryStatus.Cancelled;
                    entry.CancellationReason = request.CancellationReason;
                    entry.ModificationDate = DateTime.Now;
                    entry.ModifiedBy = userName;
                    await _entryRepository.UpdateAsync(entry, ct);

                    await _accountingPostingService.ReverseDocumentPostingAsync("InventoryEntry", entry.Id, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _entryRepository.FirstOrDefaultAsync(new GetInventoryEntryByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<InventoryEntryDto>(full);
                return new Response<InventoryEntryDto>(dto, "Entrada de inventario cancelada correctamente.");
            }
        }
    }
}
