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
    public class GetBaseProductBySlugQuery : IRequest<Response<ProductDto>>
    {
        public string Slug { get; set; } = null!;
        public bool? IsLogged { get; set; }
        public int? CustomerTypeId { get; set; }
    }
    public class GetBaseProductBySlugQueryHandler : IRequestHandler<GetBaseProductBySlugQuery, Response<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryRepositoryAsync;
        private readonly IProductPricingService _productPricingService;

        public GetBaseProductBySlugQueryHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync, IRepositoryAsync<InventoryDistribution> inventoryRepositoryAsync, IProductPricingService productPricingService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _inventoryRepositoryAsync = inventoryRepositoryAsync;
            _productPricingService = productPricingService;
        }

        public async Task<Response<ProductDto>> Handle(GetBaseProductBySlugQuery request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.FirstOrDefaultAsync(new ProductIncludesSpecification(id: null, slug: request.Slug));
            if (product == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el slug {request.Slug}");
            }
            
            // Calcular precio usando el servicio (resuelto vía lista de precios)
            product.RecomendedSalePrice = await _productPricingService.CalculateRecommendedSalePriceAsync(
                product,
                request.IsLogged ?? false,
                request.CustomerTypeId,
                ct: cancellationToken);
            product.Tax = null;
            product.CostPrice = 0;

            var dto = _mapper.Map<ProductDto>(product);
            // Disponibilidad ecommerce (físico + virtual) sin tocar CurrentStock.
            var distributions = await _inventoryRepositoryAsync.ListAsync(new FilterInventoryByProductIdsSpec(new[] { dto.Id }));
            ProductAvailabilityHelper.ApplyEcommerceStock(new[] { dto }, distributions);
            return new Response<ProductDto>(dto);
        }
    }
}
