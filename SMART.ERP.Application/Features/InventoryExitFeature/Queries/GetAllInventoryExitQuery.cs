using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryExitSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Queries
{
    public class GetAllInventoryExitQuery : IRequest<PagedResponse<List<InventoryExitDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
        public InventoryExitReason? Reason { get; set; }
        public InventoryExitStatus? Status { get; set; }
        public int? WarehouseId { get; set; }

        public class GetAllInventoryExitQueryHandler : IRequestHandler<GetAllInventoryExitQuery, PagedResponse<List<InventoryExitDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<InventoryExit> _repositoryAsync;

            public GetAllInventoryExitQueryHandler(IMapper mapper, IReadRepositoryAsync<InventoryExit> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<InventoryExitDto>>> Handle(GetAllInventoryExitQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
                }

                var exits = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationInventoryExitSpecification(request.Parameter, request.PageNumber, request.PageSize,
                        request.Reason, request.Status, request.WarehouseId), cancellationToken);
                var dto = _mapper.Map<List<InventoryExitDto>>(exits);
                return new PagedResponse<List<InventoryExitDto>>(dto, request.PageNumber, request.PageSize,
                    request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
            }
        }
    }
}
