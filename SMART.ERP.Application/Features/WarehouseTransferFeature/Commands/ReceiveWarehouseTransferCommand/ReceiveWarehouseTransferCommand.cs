using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WarehouseTransfer;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Specifications.WarehouseTransferSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.ReceiveWarehouseTransferCommand
{
    public class ReceiveWarehouseTransferCommand : IRequest<Response<WarehouseTransferDto>>
    {
        public int Id { get; set; }

        public class ReceiveWarehouseTransferCommandHandler : IRequestHandler<ReceiveWarehouseTransferCommand, Response<WarehouseTransferDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<WarehouseTransfer> _transferRepository;
            private readonly IRepositoryAsync<WarehouseTransferItem> _itemRepository;

            public ReceiveWarehouseTransferCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<WarehouseTransfer> transferRepository, IRepositoryAsync<WarehouseTransferItem> itemRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _transferRepository = transferRepository;
                _itemRepository = itemRepository;
            }

            public async Task<Response<WarehouseTransferDto>> Handle(ReceiveWarehouseTransferCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var transfer = await _transferRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una transferencia con el Id {request.Id}");

                    if (transfer.Status != WarehouseTransferStatus.Sent)
                        throw new ApiException("Solo se pueden recibir transferencias en estado Enviada.");

                    var items = await _itemRepository.ListAsync(new WarehouseTransferItemsByTransferSpecification(transfer.Id), ct);

                    foreach (var item in items)
                    {
                        await _movementService.RecordMovementAsync(new RecordMovementInput
                        {
                            ProductId = item.ProductId,
                            WarehouseId = transfer.DestinationWarehouseId,
                            MovementDate = DateTime.Now,
                            MovementType = KardexMovementType.TransferIn,
                            DocumentType = "WarehouseTransfer",
                            DocumentId = transfer.Id,
                            DocumentCode = transfer.Code,
                            QuantityIn = item.Quantity,
                            QuantityOut = 0m,
                            UnitCost = item.UnitCost,
                            UserId = userId,
                            UserName = userName,
                            SyncStock = true,
                            UpdateProductCost = false
                        }, ct);
                    }

                    transfer.Status = WarehouseTransferStatus.Received;
                    transfer.ReceivedDate = DateTime.Now;
                    transfer.ReceivedBy = userName;
                    transfer.ModificationDate = DateTime.Now;
                    transfer.ModifiedBy = userName;
                    await _transferRepository.UpdateAsync(transfer, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _transferRepository.FirstOrDefaultAsync(new GetWarehouseTransferByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<WarehouseTransferDto>(full);
                return new Response<WarehouseTransferDto>(dto, "Transferencia recibida correctamente.");
            }
        }
    }
}
