using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryMovementSpecification
{
    /// <summary>
    /// Movimientos de Kardex de un producto dentro de un rango de fechas, opcionalmente por
    /// bodega y excluyendo cancelaciones. Ordenados ascendentemente para presentar el saldo.
    /// </summary>
    public class GetMovementsByProductSpecification : Specification<InventoryMovement>
    {
        public GetMovementsByProductSpecification(int productId, int? warehouseId, DateTime? startDate, DateTime? endDate, bool includeCancellations)
        {
            Query.Where(x => x.ProductId == productId)
                 .OrderBy(x => x.MovementDate)
                 .ThenBy(x => x.Id)
                 .AsNoTracking();

            if (warehouseId.HasValue)
            {
                Query.Where(x => x.WarehouseId == warehouseId.Value);
            }
            if (startDate.HasValue)
            {
                Query.Where(x => x.MovementDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                Query.Where(x => x.MovementDate <= endDate.Value);
            }
            if (!includeCancellations)
            {
                Query.Where(x => !x.IsCancellation);
            }
        }
    }
}
