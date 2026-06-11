using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WarehouseTransfer;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Specifications.WarehouseTransferSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.SendWarehouseTransferCommand
{
    public class SendWarehouseTransferCommand : IRequest<Response<WarehouseTransferDto>>
    {
        public int Id { get; set; }

        public class SendWarehouseTransferCommandHandler : IRequestHandler<SendWarehouseTransferCommand, Response<WarehouseTransferDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<WarehouseTransfer> _transferRepository;
            private readonly IRepositoryAsync<WarehouseTransferItem> _itemRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;
            private readonly IReadRepositoryAsync<InventoryDistribution> _distributionRepository;

            public SendWarehouseTransferCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<WarehouseTransfer> transferRepository, IRepositoryAsync<WarehouseTransferItem> itemRepository,
                IReadRepositoryAsync<Prefix> prefixRepository, IReadRepositoryAsync<Product> productRepository,
                IReadRepositoryAsync<InventoryDistribution> distributionRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _transferRepository = transferRepository;
                _itemRepository = itemRepository;
                _prefixRepository = prefixRepository;
                _productRepository = productRepository;
                _distributionRepository = distributionRepository;
            }

            public async Task<Response<WarehouseTransferDto>> Handle(SendWarehouseTransferCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var transfer = await _transferRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una transferencia con el Id {request.Id}");

                    if (transfer.Status != WarehouseTransferStatus.Draft)
                        throw new ApiException("Solo se pueden enviar transferencias en estado Borrador.");

                    var items = await _itemRepository.ListAsync(new WarehouseTransferItemsByTransferSpecification(transfer.Id), ct);
                    if (items.Count == 0)
                        throw new ApiException("La transferencia no tiene productos.");

                    var prefix = await _prefixRepository.GetByIdAsync(transfer.PrefixId, ct);
                    var code = prefix != null ? InventoryCodeGenerator.Generate(prefix.Format, transfer.Id) : transfer.Code;

                    foreach (var item in items)
                    {
                        var distribution = await _distributionRepository.FirstOrDefaultAsync(
                            new FilterInventoryDistributionByProductIdAndWarehouseId(item.ProductId, transfer.OriginWarehouseId), ct);
                        var available = distribution?.Quantity ?? 0m;
                        if (available < item.Quantity)
                            throw new ApiException($"Stock insuficiente en el almacén de origen para el producto {item.ProductId}. Disponible: {available}, requerido: {item.Quantity}.");

                        var product = await _productRepository.FirstOrDefaultAsync(new ProductByIdIgnoreFiltersSpecification(item.ProductId), ct);
                        item.UnitCost = product?.CostPrice;
                        await _itemRepository.UpdateAsync(item, ct);

                        await _movementService.RecordMovementAsync(new RecordMovementInput
                        {
                            ProductId = item.ProductId,
                            WarehouseId = transfer.OriginWarehouseId,
                            MovementDate = transfer.TransferDate,
                            MovementType = KardexMovementType.TransferOut,
                            DocumentType = "WarehouseTransfer",
                            DocumentId = transfer.Id,
                            DocumentCode = code,
                            QuantityIn = 0m,
                            QuantityOut = item.Quantity,
                            UnitCost = product?.CostPrice,
                            UserId = userId,
                            UserName = userName,
                            SyncStock = true,
                            UpdateProductCost = false
                        }, ct);
                    }

                    transfer.Code = code;
                    transfer.Status = WarehouseTransferStatus.Sent;
                    transfer.SentDate = DateTime.Now;
                    transfer.SentBy = userName;
                    transfer.ModificationDate = DateTime.Now;
                    transfer.ModifiedBy = userName;
                    await _transferRepository.UpdateAsync(transfer, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _transferRepository.FirstOrDefaultAsync(new GetWarehouseTransferByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<WarehouseTransferDto>(full);
                return new Response<WarehouseTransferDto>(dto, "Transferencia enviada correctamente.");
            }
        }
    }
}
