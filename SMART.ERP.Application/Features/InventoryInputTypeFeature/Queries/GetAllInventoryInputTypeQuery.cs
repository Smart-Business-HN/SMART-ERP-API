using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInputType;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryInputTypeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputTypeFeature.Queries
{
    public class GetAllInventoryInputTypeQuery : IRequest<PagedResponse<List<InventoryInputTypeDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllInventoryInputTypeQueryHandler : IRequestHandler<GetAllInventoryInputTypeQuery, PagedResponse<List<InventoryInputTypeDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<InventoryInputType> _repositoryAsync;

            public GetAllInventoryInputTypeQueryHandler(IMapper mapper, IRepositoryAsync<InventoryInputType> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<InventoryInputTypeDto>>> Handle(GetAllInventoryInputTypeQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var inventoryInputTypes = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationInventoryInputTypeSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<InventoryInputTypeDto>>(inventoryInputTypes);
                return new PagedResponse<List<InventoryInputTypeDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
