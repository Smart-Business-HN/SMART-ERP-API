using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryDistributionSpecification
{
    public class FilterInventoryDistributionByProductIdAndWarehouseId : Specification<InventoryDistribution>
    {
        public FilterInventoryDistributionByProductIdAndWarehouseId(int productId, int warehouseId)
        {
            // IgnoreQueryFilters: solo se usa en flujos de inventario (confirmar/cancelar/validar/transferir)
            // que operan sobre documentos existentes y deben resolver el producto aunque este eliminado.
            Query.IgnoreQueryFilters().Include(x => x.Product).Where(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        }
    }
}
