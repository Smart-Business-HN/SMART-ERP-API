using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.InventoryExitSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Commands.ConfirmInventoryExitCommand
{
    public class ConfirmInventoryExitCommand : IRequest<Response<InventoryExitDto>>
    {
        public int Id { get; set; }

        public class ConfirmInventoryExitCommandHandler : IRequestHandler<ConfirmInventoryExitCommand, Response<InventoryExitDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<InventoryExit> _exitRepository;
            private readonly IRepositoryAsync<InventoryExitItem> _itemRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;
            private readonly IReadRepositoryAsync<InventoryDistribution> _distributionRepository;
            private readonly IAccountingPostingService _accountingPostingService;

            public ConfirmInventoryExitCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<InventoryExit> exitRepository, IRepositoryAsync<InventoryExitItem> itemRepository,
                IReadRepositoryAsync<Prefix> prefixRepository, IReadRepositoryAsync<Product> productRepository,
                IReadRepositoryAsync<InventoryDistribution> distributionRepository,
                IAccountingPostingService accountingPostingService)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _exitRepository = exitRepository;
                _itemRepository = itemRepository;
                _prefixRepository = prefixRepository;
                _productRepository = productRepository;
                _distributionRepository = distributionRepository;
                _accountingPostingService = accountingPostingService;
            }

            public async Task<Response<InventoryExitDto>> Handle(ConfirmInventoryExitCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var exit = await _exitRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una salida de inventario con el Id {request.Id}");

                    if (exit.Status != InventoryExitStatus.Draft)
                        throw new ApiException("Solo se pueden confirmar salidas en estado Borrador.");

                    var items = await _itemRepository.ListAsync(new InventoryExitItemsByExitSpecification(exit.Id), ct);
                    if (items.Count == 0)
                        throw new ApiException("La salida no tiene productos para confirmar.");

                    var prefix = await _prefixRepository.GetByIdAsync(exit.PrefixId, ct);
                    var code = prefix != null ? InventoryCodeGenerator.Generate(prefix.Format, exit.Id) : exit.Code;
                    var movementType = _movementService.MapExitReasonToMovementType(exit.ExitReason);

                    foreach (var item in items)
                    {
                        var distribution = await _distributionRepository.FirstOrDefaultAsync(
                            new FilterInventoryDistributionByProductIdAndWarehouseId(item.ProductId, exit.WarehouseId), ct);
                        var available = distribution?.Quantity ?? 0m;
                        if (available < item.Quantity)
                            throw new ApiException($"Stock insuficiente para el producto {item.ProductId}. Disponible: {available}, requerido: {item.Quantity}.");

                        var product = await _productRepository.GetByIdAsync(item.ProductId, ct);
                        item.PreviousStock = available;
                        item.UnitCost = product?.CostPrice;
                        await _itemRepository.UpdateAsync(item, ct);

                        await _movementService.RecordMovementAsync(new RecordMovementInput
                        {
                            ProductId = item.ProductId,
                            WarehouseId = exit.WarehouseId,
                            MovementDate = exit.ExitDate,
                            MovementType = movementType,
                            DocumentType = "InventoryExit",
                            DocumentId = exit.Id,
                            DocumentCode = code,
                            ThirdPartyName = exit.BeneficiaryName,
                            QuantityIn = 0m,
                            QuantityOut = item.Quantity,
                            UnitCost = product?.CostPrice,
                            UserId = userId,
                            UserName = userName,
                            SyncStock = true,
                            UpdateProductCost = false
                        }, ct);
                    }

                    foreach (var productId in items.Select(i => i.ProductId).Distinct())
                    {
                        await _movementService.SyncProductStockAsync(productId, ct);
                    }

                    exit.Code = code;
                    exit.Status = InventoryExitStatus.Confirmed;
                    exit.ConfirmedDate = DateTime.Now;
                    exit.ConfirmedBy = userName;
                    exit.ModificationDate = DateTime.Now;
                    exit.ModifiedBy = userName;
                    await _exitRepository.UpdateAsync(exit, ct);

                    // Posteo contable dentro de la misma transacción: si el asiento falla (cuenta mal
                    // configurada, período cerrado, etc.) se revierte también la rebaja de stock y la
                    // salida queda en Borrador, evitando kardex sin asiento. Usa el mismo DbContext.
                    await _accountingPostingService.PostInventoryExitAsync(exit.Id, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _exitRepository.FirstOrDefaultAsync(new GetInventoryExitByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<InventoryExitDto>(full);
                return new Response<InventoryExitDto>(dto, "Salida de inventario confirmada correctamente.");
            }
        }
    }
}
