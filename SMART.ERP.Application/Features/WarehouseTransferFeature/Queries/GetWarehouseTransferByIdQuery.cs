using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WarehouseTransfer;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WarehouseTransferSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Queries
{
    public class GetWarehouseTransferByIdQuery : IRequest<Response<WarehouseTransferDto>>
    {
        public int Id { get; set; }

        public class GetWarehouseTransferByIdQueryHandler : IRequestHandler<GetWarehouseTransferByIdQuery, Response<WarehouseTransferDto>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<WarehouseTransfer> _repositoryAsync;

            public GetWarehouseTransferByIdQueryHandler(IMapper mapper, IReadRepositoryAsync<WarehouseTransfer> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<WarehouseTransferDto>> Handle(GetWarehouseTransferByIdQuery request, CancellationToken cancellationToken)
            {
                var transfer = await _repositoryAsync.FirstOrDefaultAsync(new GetWarehouseTransferByIdSpecification(request.Id), cancellationToken)
                    ?? throw new ApiException($"No existe una transferencia con el Id {request.Id}");
                var dto = _mapper.Map<WarehouseTransferDto>(transfer);
                return new Response<WarehouseTransferDto>(dto);
            }
        }
    }
}
