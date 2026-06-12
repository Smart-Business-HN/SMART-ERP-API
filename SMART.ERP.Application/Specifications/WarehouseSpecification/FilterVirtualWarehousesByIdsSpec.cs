using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseSpecification
{
    /// <summary>
    /// Devuelve, dentro de un conjunto de Ids de almacén, solo los que son virtuales
    /// (consignados/dropshipping). Se usa para excluir el stock virtual de Product.CurrentStock.
    /// </summary>
    public class FilterVirtualWarehousesByIdsSpec : Specification<Warehouse>
    {
        public FilterVirtualWarehousesByIdsSpec(IEnumerable<int> warehouseIds)
        {
            Query.Where(w => warehouseIds.Contains(w.Id) && w.IsVirtual).AsNoTracking();
        }
    }
}
