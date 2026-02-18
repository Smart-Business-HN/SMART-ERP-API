using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.ShippingCostCalculator;
using SMART.ERP.Application.Services.WarehouseSelection;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Services
{
    public class ProductPricingService : IProductPricingService
    {
        private readonly IRepositoryAsync<Warehouse>? _warehouseRepository;
        private readonly IShippingCostCalculatorService? _shippingCalculator;
        private readonly IWarehouseSelectionService? _warehouseSelectionService;

        public ProductPricingService()
        {
        }

        public ProductPricingService(
            IRepositoryAsync<Warehouse> warehouseRepository,
            IShippingCostCalculatorService shippingCalculator,
            IWarehouseSelectionService warehouseSelectionService)
        {
            _warehouseRepository = warehouseRepository;
            _shippingCalculator = shippingCalculator;
            _warehouseSelectionService = warehouseSelectionService;
        }

        public decimal CalculateRecommendedSalePrice(Product product, bool isUserSignedIn, int? customerTypeId = null)
        {
            if (!isUserSignedIn)
            {
                // Usuario no autenticado: margen del 30%
                return Math.Ceiling((product.CostPrice * (decimal)1.3) * (1 + (product.Tax!.Rate / 100)));
            }

            // Usuario autenticado: calcular según tipo de cliente
            decimal marginMultiplier = GetMarginMultiplierByCustomerType(customerTypeId);
            return Math.Ceiling((product.CostPrice * marginMultiplier) * (1 + (product.Tax!.Rate / 100)));
        }

        public async Task<ProductPriceWithShippingDto> CalculateRecommendedSalePriceWithShippingAsync(
            Product product,
            bool isUserSignedIn,
            int? customerTypeId = null,
            int? sourceWarehouseId = null,
            int? destinationCityId = null)
        {
            if (_warehouseRepository == null || _shippingCalculator == null || _warehouseSelectionService == null)
            {
                throw new InvalidOperationException("Este método requiere inyección de dependencias completa");
            }

            var basePrice = CalculateRecommendedSalePrice(product, isUserSignedIn, customerTypeId);

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

        private decimal GetMarginMultiplierByCustomerType(int? customerTypeId)
        {
            if (!customerTypeId.HasValue)
            {
                // Si no se especifica tipo de cliente, usar margen por defecto del 30%
                return (decimal)1.3;
            }

            return customerTypeId switch
            {
                (int)CustomerTypeEnum.Basico => (decimal)1.25,      // 25% de margen
                (int)CustomerTypeEnum.Recurrente => (decimal)1.18,  // 18% de margen
                (int)CustomerTypeEnum.Mayorista => (decimal)1.08,   // 8% de margen
                (int)CustomerTypeEnum.Integrador => (decimal)1.1,   // 10% de margen
                (int)CustomerTypeEnum.Corporativo => (decimal)1.1,  // 10% de margen
                (int)CustomerTypeEnum.Empleado => (decimal)1.05,    // 5% de margen (descuento especial)
                _ => (decimal)1.2  // Margen por defecto del 20% para tipos no reconocidos
            };
        }
    }
}
