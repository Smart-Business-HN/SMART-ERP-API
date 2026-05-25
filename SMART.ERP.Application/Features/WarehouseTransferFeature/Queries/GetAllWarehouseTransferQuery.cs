using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WarehouseTransfer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WarehouseTransferSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Queries
{
    public class GetAllWarehouseTransferQuery : IRequest<PagedResponse<List<WarehouseTransferDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
        public WarehouseTransferStatus? Status { get; set; }
        public int? OriginWarehouseId { get; set; }
        public int? DestinationWarehouseId { get; set; }

        public class GetAllWarehouseTransferQueryHandler : IRequestHandler<GetAllWarehouseTransferQuery, PagedResponse<List<WarehouseTransferDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<WarehouseTransfer> _repositoryAsync;

            public GetAllWarehouseTransferQueryHandler(IMapper mapper, IReadRepositoryAsync<WarehouseTransfer> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<WarehouseTransferDto>>> Handle(GetAllWarehouseTransferQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
                }

                var transfers = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationWarehouseTransferSpecification(request.Parameter, request.PageNumber, request.PageSize,
                        request.Status, request.OriginWarehouseId, request.DestinationWarehouseId), cancellationToken);
                var dto = _mapper.Map<List<WarehouseTransferDto>>(transfers);
                return new PagedResponse<List<WarehouseTransferDto>>(dto, request.PageNumber, request.PageSize,
                    request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
            }
        }
    }
}
