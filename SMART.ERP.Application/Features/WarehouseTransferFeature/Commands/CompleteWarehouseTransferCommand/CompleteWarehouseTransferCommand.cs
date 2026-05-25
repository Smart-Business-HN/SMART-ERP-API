using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WarehouseTransfer;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.WarehouseTransferSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.CompleteWarehouseTransferCommand
{
    public class CompleteWarehouseTransferCommand : IRequest<Response<WarehouseTransferDto>>
    {
        public int Id { get; set; }

        public class CompleteWarehouseTransferCommandHandler : IRequestHandler<CompleteWarehouseTransferCommand, Response<WarehouseTransferDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<WarehouseTransfer> _transferRepository;

            public CompleteWarehouseTransferCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<WarehouseTransfer> transferRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _transferRepository = transferRepository;
            }

            public async Task<Response<WarehouseTransferDto>> Handle(CompleteWarehouseTransferCommand request, CancellationToken cancellationToken)
            {
                var transfer = await _transferRepository.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una transferencia con el Id {request.Id}");

                if (transfer.Status != WarehouseTransferStatus.Received)
                    throw new ApiException("Solo se pueden completar transferencias en estado Recibida.");

                transfer.Status = WarehouseTransferStatus.Completed;
                transfer.ModificationDate = DateTime.Now;
                transfer.ModifiedBy = _jwtService.GetSubjectToken();
                await _transferRepository.UpdateAsync(transfer, cancellationToken);
                await _transferRepository.SaveChangesAsync(cancellationToken);

                var full = await _transferRepository.FirstOrDefaultAsync(new GetWarehouseTransferByIdSpecification(request.Id), cancellationToken);
                var dto = _mapper.Map<WarehouseTransferDto>(full);
                return new Response<WarehouseTransferDto>(dto, "Transferencia completada correctamente.");
            }
        }
    }
}
