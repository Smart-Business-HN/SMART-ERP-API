using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Services.InventoryMovementService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.ProductCacheInvalidator;
using SMART.ERP.Application.Specifications.InventoryExitSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Commands.CancelInventoryExitCommand
{
    public class CancelInventoryExitCommand : IRequest<Response<InventoryExitDto>>
    {
        public int Id { get; set; }
        public string? CancellationReason { get; set; }

        public class CancelInventoryExitCommandHandler : IRequestHandler<CancelInventoryExitCommand, Response<InventoryExitDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IInventoryMovementService _movementService;
            private readonly IProductCacheInvalidator _cacheInvalidator;
            private readonly IRepositoryAsync<InventoryExit> _exitRepository;
            private readonly IAccountingPostingService _accountingPostingService;

            public CancelInventoryExitCommandHandler(IMapper mapper, IJwtService jwtService, IUnitOfWork unitOfWork,
                IInventoryMovementService movementService, IProductCacheInvalidator cacheInvalidator,
                IRepositoryAsync<InventoryExit> exitRepository, IAccountingPostingService accountingPostingService)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _unitOfWork = unitOfWork;
                _movementService = movementService;
                _cacheInvalidator = cacheInvalidator;
                _exitRepository = exitRepository;
                _accountingPostingService = accountingPostingService;
            }

            public async Task<Response<InventoryExitDto>> Handle(CancelInventoryExitCommand request, CancellationToken cancellationToken)
            {
                var userName = _jwtService.GetSubjectToken();
                Guid? userId = _jwtService.GetUidToken();

                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    var exit = await _exitRepository.GetByIdAsync(request.Id, ct)
                        ?? throw new ApiException($"No existe una salida de inventario con el Id {request.Id}");

                    if (exit.Status != InventoryExitStatus.Confirmed)
                        throw new ApiException("Solo se pueden cancelar salidas confirmadas.");

                    // Revertir Kardex y stock (las salidas vuelven a sumar stock).
                    await _movementService.RecordCancellationForDocumentAsync(
                        "InventoryExit", exit.Id, DateTime.Now, KardexMovementType.InventoryExitCancellation, userId, userName, ct);

                    exit.Status = InventoryExitStatus.Cancelled;
                    exit.CancellationReason = request.CancellationReason;
                    exit.ModificationDate = DateTime.Now;
                    exit.ModifiedBy = userName;
                    await _exitRepository.UpdateAsync(exit, ct);

                    await _accountingPostingService.ReverseDocumentPostingAsync("InventoryExit", exit.Id, ct);
                }, cancellationToken);

                await _cacheInvalidator.InvalidateAsync(cancellationToken);

                var full = await _exitRepository.FirstOrDefaultAsync(new GetInventoryExitByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<InventoryExitDto>(full);
                return new Response<InventoryExitDto>(dto, "Salida de inventario cancelada correctamente.");
            }
        }
    }
}
