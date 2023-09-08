using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryInputSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InventoryInputFeature.Queries
{
    public class GetAllInventoryInputQuery : IRequest<PagedResponse<List<InventoryInputDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllInventoryInputQueryHandler : IRequestHandler<GetAllInventoryInputQuery, PagedResponse<List<InventoryInputDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<InventoryInput> _repositoryAsync;

            public GetAllInventoryInputQueryHandler(IMapper mapper, IRepositoryAsync<InventoryInput> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<InventoryInputDto>>> Handle(GetAllInventoryInputQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var inventoryInputTypes = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationInventoryInputSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<InventoryInputDto>>(inventoryInputTypes);
                return new PagedResponse<List<InventoryInputDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
