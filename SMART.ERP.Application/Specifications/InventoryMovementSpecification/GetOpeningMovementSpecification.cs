using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryMovementSpecification
{
    /// <summary>
    /// Último movimiento estrictamente anterior a la fecha de inicio, para calcular el saldo
    /// inicial del Kardex.
    /// </summary>
    public class GetOpeningMovementSpecification : Specification<InventoryMovement>
    {
        public GetOpeningMovementSpecification(int productId, int? warehouseId, DateTime startDate)
        {
            Query.Where(x => x.ProductId == productId && x.MovementDate < startDate)
                 .OrderByDescending(x => x.MovementDate)
                 .ThenByDescending(x => x.Id)
                 .AsNoTracking();

            if (warehouseId.HasValue)
            {
                Query.Where(x => x.WarehouseId == warehouseId.Value);
            }
        }
    }
}
