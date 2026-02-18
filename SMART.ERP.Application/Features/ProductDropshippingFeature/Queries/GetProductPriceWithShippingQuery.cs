using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProductDropshippingFeature.Queries
{
    public class GetProductPriceWithShippingQuery : IRequest<Response<ProductPriceWithShippingDto>>
    {
        public int ProductId { get; set; }
        public bool IsUserSignedIn { get; set; }
        public int? CustomerTypeId { get; set; }
        public int? DestinationCityId { get; set; }

        public class GetProductPriceWithShippingQueryHandler : IRequestHandler<GetProductPriceWithShippingQuery, Response<ProductPriceWithShippingDto>>
        {
            private readonly IRepositoryAsync<Product> _productRepository;
            private readonly IProductPricingService _pricingService;

            public GetProductPriceWithShippingQueryHandler(
                IRepositoryAsync<Product> productRepository,
                IProductPricingService pricingService)
            {
                _productRepository = productRepository;
                _pricingService = pricingService;
            }

            public async Task<Response<ProductPriceWithShippingDto>> Handle(GetProductPriceWithShippingQuery request, CancellationToken cancellationToken)
            {
                var spec = new FilterProductByIdWithInventorySpec(request.ProductId);
                var product = await _productRepository.FirstOrDefaultAsync(spec);

                if (product == null)
                {
                    throw new ApiException($"Producto con ID {request.ProductId} no encontrado");
                }

                var priceWithShipping = await _pricingService.CalculateRecommendedSalePriceWithShippingAsync(
                    product,
                    request.IsUserSignedIn,
                    request.CustomerTypeId,
                    null, // Let the service select the optimal warehouse
                    request.DestinationCityId);

                return new Response<ProductPriceWithShippingDto>(priceWithShipping);
            }
        }
    }
}
