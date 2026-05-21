using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Services.ShippingCostCalculator;
using SMART.ERP.Application.Services.WarehouseSelection;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services
{
    public class ProductPricingService : IProductPricingService
    {
        private readonly IPriceListService _priceListService;
        private readonly IRepositoryAsync<Warehouse>? _warehouseRepository;
        private readonly IShippingCostCalculatorService? _shippingCalculator;
        private readonly IWarehouseSelectionService? _warehouseSelectionService;

        public ProductPricingService(IPriceListService priceListService)
        {
            _priceListService = priceListService;
        }

        public ProductPricingService(
            IPriceListService priceListService,
            IRepositoryAsync<Warehouse> warehouseRepository,
            IShippingCostCalculatorService shippingCalculator,
            IWarehouseSelectionService warehouseSelectionService)
        {
            _priceListService = priceListService;
            _warehouseRepository = warehouseRepository;
            _shippingCalculator = shippingCalculator;
            _warehouseSelectionService = warehouseSelectionService;
        }

        public async Task<decimal> CalculateRecommendedSalePriceAsync(
            Product product,
            bool isUserSignedIn,
            int? customerTypeId = null,
            Guid? customerId = null,
            CancellationToken ct = default)
        {
            var listId = await _priceListService.ResolvePriceListIdAsync(
                customerId: customerId,
                customerTypeId: customerTypeId,
                isAnonymous: !isUserSignedIn,
                ct: ct);

            var price = await _priceListService.GetPriceAsync(product.Id, listId, ct);
            return price ?? 0m;
        }

        public async Task<IReadOnlyDictionary<int, decimal>> CalculateRecommendedSalePricesAsync(
            IEnumerable<int> productIds,
            bool isUserSignedIn,
            int? customerTypeId = null,
            Guid? customerId = null,
            CancellationToken ct = default)
        {
            var listId = await _priceListService.ResolvePriceListIdAsync(
                customerId: customerId,
                customerTypeId: customerTypeId,
                isAnonymous: !isUserSignedIn,
                ct: ct);

            return await _priceListService.GetPricesAsync(productIds, listId, ct);
        }

        public async Task<ProductPriceWithShippingDto> CalculateRecommendedSalePriceWithShippingAsync(
            Product product,
            bool isUserSignedIn,
            int? customerTypeId = null,
            int? sourceWarehouseId = null,
            int? destinationCityId = null,
            Guid? customerId = null,
            CancellationToken ct = default)
        {
            if (_warehouseRepository == null || _shippingCalculator == null || _warehouseSelectionService == null)
            {
                throw new InvalidOperationException("Este método requiere inyección de dependencias completa");
            }

            var basePrice = await CalculateRecommendedSalePriceAsync(product, isUserSignedIn, customerTypeId, customerId, ct);

            if (!sourceWarehouseId.HasValue)
            {
                var selection = await _warehouseSelectionService
                    .SelectOptimalWarehouseAsync(product.Id, 1, destinationCityId);
                sourceWarehouseId = selection.WarehouseId;
            }

            var shippingCost = 0m;
            var warehouse = await _warehouseRepository.GetByIdAsync(sourceWarehouseId.Value);

            if (warehouse?.IsVirtual == true)
            {
                var shippingResult = await _shippingCalculator
                    .CalculateShippingCostAsync(product.Id, sourceWarehouseId, destinationCityId);
                shippingCost = shippingResult.DefaultCost;
            }

            return new ProductPriceWithShippingDto
            {
                BasePrice = basePrice,
                ShippingCost = shippingCost,
                TotalPrice = basePrice + shippingCost,
                IsFromVirtualStock = shippingCost > 0,
                SourceWarehouseId = sourceWarehouseId,
                SourceWarehouseName = warehouse?.Name,
                ProviderName = warehouse?.ProviderWarehouses?.FirstOrDefault()?.Provider?.Name
            };
        }
    }
}
