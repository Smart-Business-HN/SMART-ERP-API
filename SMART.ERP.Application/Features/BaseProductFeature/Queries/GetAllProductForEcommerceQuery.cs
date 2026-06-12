using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
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
        public bool? IsUserSignIn { get; set; }
        public int? CustomerTypeId { get; set; }
        public class GetAllProductForEcommerceQueryHandler : IRequestHandler<GetAllProductForEcommerceQuery, PagedResponse<List<ProductDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Product> _repositoryAsync;
            private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
            private readonly IRepositoryAsync<InventoryDistribution> _inventoryRepositoryAsync;
            private readonly IProductPricingService _productPricingService;

            public GetAllProductForEcommerceQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,
                IRepositoryAsync<Category> categoryRepositoryAsync, IRepositoryAsync<InventoryDistribution> inventoryRepositoryAsync,
                IProductPricingService productPricingService)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
                _categoryRepositoryAsync = categoryRepositoryAsync;
                _inventoryRepositoryAsync = inventoryRepositoryAsync;
                _productPricingService = productPricingService;
            }
            public async Task<PagedResponse<List<ProductDto>>> Handle(GetAllProductForEcommerceQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }
                var categories = await _categoryRepositoryAsync.ListAsync();
                var products = await _repositoryAsync.ListAsync(new FilterAndPaginationProductForEcommerceSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                
                // Calcular precios en batch usando el servicio (evita N+1)
                var prices = await _productPricingService.CalculateRecommendedSalePricesAsync(
                    products.Select(p => p.Id),
                    request.IsUserSignIn ?? false,
                    request.CustomerTypeId,
                    ct: cancellationToken);
                foreach (var item in products)
                {
                    item.RecomendedSalePrice = prices.GetValueOrDefault(item.Id, 0);
                    item.CostPrice = 0;
                    item.Tax = null;
                }
                var dto = _mapper.Map<List<ProductDto>>(products);
                var spec = new ProductsForEcommerceSpecification();
                foreach (var product in dto)
                {
                    product.SubCategory!.Category = _mapper.Map<CategoryDto>(categories.Find(y => y.Id == product.SubCategory.CategoryId));
                }
                // Disponibilidad ecommerce (físico + virtual) sin tocar CurrentStock.
                var distributions = await _inventoryRepositoryAsync.ListAsync(new FilterInventoryByProductIdsSpec(dto.Select(d => d.Id).ToList()));
                ProductAvailabilityHelper.ApplyEcommerceStock(dto, distributions);
                return new PagedResponse<List<ProductDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync(spec));
            }
        }
    }
}
