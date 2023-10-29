using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllProductForEcommerceQuery : IRequest<PagedResponse<List<ProductDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllProductForEcommerceQueryHandler : IRequestHandler<GetAllProductForEcommerceQuery, PagedResponse<List<ProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;
            private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

            public GetAllProductForEcommerceQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,
                IRepositoryAsync<Category> categoryRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _categoryRepositoryAsync = categoryRepositoryAsync;
            }
            public async Task<PagedResponse<List<ProductDto>>> Handle(GetAllProductForEcommerceQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var categories = await _categoryRepositoryAsync.ListAsync();
                var products = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationProductForEcommerceSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<ProductDto>>(products);
                foreach (var product in dto)
                {
                    product.SubCategory!.Category = _mapper.Map<CategoryDto>(categories.Find(y => y.Id == product.SubCategory.CategoryId));
                }
                return new PagedResponse<List<ProductDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
