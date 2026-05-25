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

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.CancelWarehouseTransferCommand
{
    public class CancelWarehouseTransferCommand : IRequest<Response<WarehouseTransferDto>>
    {
        public int Id { get; set; }
        public string? CancellationReason { get; set; }

        public class CancelWarehouseTransferCommandHandler : IRequestHandler<CancelWarehouseTransferCommand, Response<WarehouseTransferDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<WarehouseTransfer> _transferRepository;

            public CancelWarehouseTransferCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<WarehouseTransfer> transferRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _transferRepository = transferRepository;
            }

            public async Task<Response<WarehouseTransferDto>> Handle(CancelWarehouseTransferCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var transfer = await _transferRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una transferencia con el Id {request.Id}");

                    if (transfer.Status == WarehouseTransferStatus.Completed)
                        throw new ApiException("No se puede cancelar una transferencia completada.");
                    if (transfer.Status == WarehouseTransferStatus.Cancelled)
                        throw new ApiException("La transferencia ya está cancelada.");

                    // Revertir cualquier movimiento de stock generado (TransferOut/TransferIn).
                    if (transfer.Status is WarehouseTransferStatus.Sent or WarehouseTransferStatus.Received)
                    {
                        await _movementService.RecordCancellationForDocumentAsync(
                            "WarehouseTransfer", transfer.Id, DateTime.Now, KardexMovementType.TransferCancellation, userId, userName, ct);
                    }

                    transfer.Status = WarehouseTransferStatus.Cancelled;
                    transfer.CancellationReason = request.CancellationReason;
                    transfer.ModificationDate = DateTime.Now;
                    transfer.ModifiedBy = userName;
                    await _transferRepository.UpdateAsync(transfer, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _transferRepository.FirstOrDefaultAsync(new GetWarehouseTransferByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<WarehouseTransferDto>(full);
                return new Response<WarehouseTransferDto>(dto, "Transferencia cancelada correctamente.");
            }
        }
    }
}
