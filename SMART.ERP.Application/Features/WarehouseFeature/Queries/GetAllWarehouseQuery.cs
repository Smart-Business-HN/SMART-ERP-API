using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WarehouseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WarehouseFeature.Queries
{
    public class GetAllWarehouseQuery : IRequest<PagedResponse<List<WarehouseDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllWarehouseQueryHandler : IRequestHandler<GetAllWarehouseQuery, PagedResponse<List<WarehouseDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Warehouse> _repositoryAsync;

            public GetAllWarehouseQueryHandler(IMapper mapper, IRepositoryAsync<Warehouse> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<WarehouseDto>>> Handle(GetAllWarehouseQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var warehouses = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationWarehouseSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<WarehouseDto>>(warehouses);
                return new PagedResponse<List<WarehouseDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
