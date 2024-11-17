using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Queries
{
    public class GetAllBaseProductsByCategorySlugQuery : IRequest<PagedResponse<List<ProductDto>>>
    {
        public string CategorySlug { get; set; } = null!;
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool? IsUserSignIn { get; set; }
        public int? CustomerTypeId { get; set; }
        public class GetAllBaseProductsQueryHandler : IRequestHandler<GetAllBaseProductsByCategorySlugQuery, PagedResponse<List<ProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;
            private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;

            public GetAllBaseProductsQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,
                IRepositoryAsync<Category> categoryRepositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _categoryRepositoryAsync = categoryRepositoryAsync;
            }
            public async Task<PagedResponse<List<ProductDto>>> Handle(GetAllBaseProductsByCategorySlugQuery request, CancellationToken cancellationToken)
            {
                var checkCategory = await _categoryRepositoryAsync.FirstOrDefaultAsync(new GetCategoryBySlugSpecification(request.CategorySlug));
                if (checkCategory == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el slug {request.CategorySlug}");
                }
                var products = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationProductsByCategorySlugSpecification(request.CategorySlug, request.Parameter, request.PageNumber - 1, request.PageSize, request.Order, request.Column));
                if (request.IsUserSignIn.HasValue && request.IsUserSignIn.Value)
                {
                    foreach (var item in products)
                    {
                        item.RecomendedSalePrice = Math.Ceiling((item.CostPrice * (decimal)1.2) * (1 + (item.Tax!.Rate / 100)));
                        item.CostPrice = 0;
                        item.Tax = null;
                    }
                }
                else
                {
                    foreach (var item in products)
                    {
                        item.RecomendedSalePrice = Math.Ceiling((item.CostPrice * (decimal)1.3) * (1 + (item.Tax!.Rate / 100)));
                        item.CostPrice = 0;
                        item.Tax = null;
                    }
                }
                var dto = _mapper.Map<List<ProductDto>>(products);
                return new PagedResponse<List<ProductDto>>(dto, request.PageNumber, request.PageSize, false ? request.PageSize : await _repositoryAsync.CountAsync(new FilterAndPaginationProductsByCategorySlugSpecification(request.CategorySlug, request.Parameter, 0, 0, request.Order, request.Column)));
            }
        }
    }
}
