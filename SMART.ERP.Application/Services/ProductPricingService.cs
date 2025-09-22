using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Services
{
    public class ProductPricingService : IProductPricingService
    {
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
