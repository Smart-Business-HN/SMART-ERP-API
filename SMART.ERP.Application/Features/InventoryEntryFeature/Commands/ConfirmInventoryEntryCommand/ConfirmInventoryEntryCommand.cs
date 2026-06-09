using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Commands.ConfirmInventoryEntryCommand
{
    public class ConfirmInventoryEntryCommand : IRequest<Response<InventoryEntryDto>>
    {
        public int Id { get; set; }

        public class ConfirmInventoryEntryCommandHandler : IRequestHandler<ConfirmInventoryEntryCommand, Response<InventoryEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<InventoryEntry> _entryRepository;
            private readonly IRepositoryAsync<InventoryEntryItem> _itemRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Provider> _providerRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;
            private readonly IReadRepositoryAsync<InventoryDistribution> _distributionRepository;
            private readonly IAccountingPostingService _accountingPostingService;

            public ConfirmInventoryEntryCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<InventoryEntry> entryRepository, IRepositoryAsync<InventoryEntryItem> itemRepository,
                IReadRepositoryAsync<Prefix> prefixRepository, IReadRepositoryAsync<Provider> providerRepository,
                IReadRepositoryAsync<Product> productRepository, IReadRepositoryAsync<InventoryDistribution> distributionRepository,
                IAccountingPostingService accountingPostingService)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _entryRepository = entryRepository;
                _itemRepository = itemRepository;
                _prefixRepository = prefixRepository;
                _providerRepository = providerRepository;
                _productRepository = productRepository;
                _distributionRepository = distributionRepository;
                _accountingPostingService = accountingPostingService;
            }

            public async Task<Response<InventoryEntryDto>> Handle(ConfirmInventoryEntryCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var entry = await _entryRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una entrada de inventario con el Id {request.Id}");

                    if (entry.Status != InventoryEntryStatus.Draft)
                        throw new ApiException("Solo se pueden confirmar entradas en estado Borrador.");

                    var items = await _itemRepository.ListAsync(new InventoryEntryItemsByEntrySpecification(entry.Id), ct);
                    if (items.Count == 0)
                        throw new ApiException("La entrada no tiene productos para confirmar.");

                    var prefix = await _prefixRepository.GetByIdAsync(entry.PrefixId, ct);
                    var code = prefix != null ? InventoryCodeGenerator.Generate(prefix.Format, entry.Id) : entry.Code;

                    string? providerName = entry.ProviderId.HasValue
                        ? (await _providerRepository.GetByIdAsync(entry.ProviderId.Value, ct))?.Name
                        : null;

                    // Solo el Ajuste de conteo físico es absoluto (fija el saldo a la cantidad contada).
                    // El Inventario inicial (OpeningStock) es ADITIVO: suma lo declarado. Tratarlo como
                    // absoluto inflaba la cantidad cuando ya existía stock (p.ej. ventas registradas antes
                    // de cargar una apertura retro-fechada), p.ej. ingresar 2 y postear 5 = 2 - (-3).
                    var isAbsolute = entry.EntryType is InventoryEntryType.Adjustment;
                    var movementType = entry.EntryType switch
                    {
                        InventoryEntryType.Purchase => KardexMovementType.Purchase,
                        InventoryEntryType.OpeningStock => KardexMovementType.OpeningStock,
                        InventoryEntryType.ProjectSurplus => KardexMovementType.ProjectSurplus,
                        _ => KardexMovementType.Adjustment
                    };

                    foreach (var item in items)
                    {
                        var distribution = await _distributionRepository.FirstOrDefaultAsync(
                            new FilterInventoryDistributionByProductIdAndWarehouseId(item.ProductId, entry.WarehouseId), ct);
                        var previousStock = distribution?.Quantity ?? 0m;
                        var product = await _productRepository.GetByIdAsync(item.ProductId, ct);

                        item.PreviousStock = previousStock;
                        item.PreviousCostPrice = product?.CostPrice ?? 0m;

                        decimal qtyIn = 0m, qtyOut = 0m;
                        if (isAbsolute)
                        {
                            var delta = item.Quantity - previousStock;
                            if (delta >= 0) qtyIn = delta; else qtyOut = -delta;
                        }
                        else
                        {
                            qtyIn = item.Quantity;
                        }

                        if (qtyIn > 0 || qtyOut > 0)
                        {
                            await _movementService.RecordMovementAsync(new RecordMovementInput
                            {
                                ProductId = item.ProductId,
                                WarehouseId = entry.WarehouseId,
                                MovementDate = entry.EntryDate,
                                MovementType = movementType,
                                DocumentType = "InventoryEntry",
                                DocumentId = entry.Id,
                                DocumentCode = code,
                                ThirdPartyName = providerName,
                                QuantityIn = qtyIn,
                                QuantityOut = qtyOut,
                                UnitCost = item.UnitCost,
                                UserId = userId,
                                UserName = userName,
                                SyncStock = true,
                                UpdateProductCost = true
                            }, ct);
                        }

                        await _itemRepository.UpdateAsync(item, ct);
                    }

                    // Resincronizar el stock total de cada producto desde sus distribuciones,
                    // aunque no se haya generado movimiento (delta cero o datos heredados).
                    foreach (var productId in items.Select(i => i.ProductId).Distinct())
                    {
                        await _movementService.SyncProductStockAsync(productId, ct);
                    }

                    entry.Code = code;
                    entry.Status = InventoryEntryStatus.Confirmed;
                    entry.ConfirmedDate = DateTime.Now;
                    entry.ConfirmedBy = userName;
                    entry.ModificationDate = DateTime.Now;
                    entry.ModifiedBy = userName;
                    await _entryRepository.UpdateAsync(entry, ct);

                    // Posteo contable dentro de la misma transacción: si el asiento falla se revierte
                    // también la entrada de stock y la entrada queda en Borrador (mismo DbContext).
                    await _accountingPostingService.PostInventoryEntryAsync(entry.Id, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _entryRepository.FirstOrDefaultAsync(new GetInventoryEntryByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<InventoryEntryDto>(full);
                return new Response<InventoryEntryDto>(dto, "Entrada de inventario confirmada correctamente.");
            }
        }
    }
}
