using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Queries
{
    public class GetAllInventoryEntryQuery : IRequest<PagedResponse<List<InventoryEntryDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
        public InventoryEntryType? EntryType { get; set; }
        public InventoryEntryStatus? Status { get; set; }
        public int? WarehouseId { get; set; }

        public class GetAllInventoryEntryQueryHandler : IRequestHandler<GetAllInventoryEntryQuery, PagedResponse<List<InventoryEntryDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<InventoryEntry> _repositoryAsync;

            public GetAllInventoryEntryQueryHandler(IMapper mapper, IReadRepositoryAsync<InventoryEntry> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<InventoryEntryDto>>> Handle(GetAllInventoryEntryQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
                }

                var entries = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationInventoryEntrySpecification(request.Parameter, request.PageNumber, request.PageSize,
                        request.EntryType, request.Status, request.WarehouseId), cancellationToken);
                var dto = _mapper.Map<List<InventoryEntryDto>>(entries);
                return new PagedResponse<List<InventoryEntryDto>>(dto, request.PageNumber, request.PageSize,
                    request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
            }
        }
    }
}
