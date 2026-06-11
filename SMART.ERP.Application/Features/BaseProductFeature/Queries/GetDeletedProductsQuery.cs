using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetDeletedProductsQuery : IRequest<PagedResponse<List<DeletedProductDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetDeletedProductsQueryHandler : IRequestHandler<GetDeletedProductsQuery, PagedResponse<List<DeletedProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;

            public GetDeletedProductsQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<PagedResponse<List<DeletedProductDto>>> Handle(GetDeletedProductsQuery request, CancellationToken cancellationToken)
            {
                var totalCount = await _repositoryAsync.CountAsync(new DeletedProductsCountSpecification(request.Parameter), cancellationToken);

                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = totalCount;
                }

                var products = await _repositoryAsync.ListAsync(
                    new DeletedProductsSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column),
                    cancellationToken);

                var dto = _mapper.Map<List<DeletedProductDto>>(products);
                return new PagedResponse<List<DeletedProductDto>>(dto, request.PageNumber, request.PageSize, totalCount);
            }
        }
    }
}
