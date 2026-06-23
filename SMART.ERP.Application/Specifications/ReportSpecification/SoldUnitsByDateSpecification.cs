using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    /// <summary>
    /// Lineas de venta (ProductSold) cuyas facturas caen dentro del rango de fechas.
    /// Se usa para agregar las unidades vendidas por producto.
    /// IgnoreQueryFilters: el historico de ventas debe contar productos eliminados luego.
    /// </summary>
    public class SoldUnitsByDateSpecification : Specification<ProductSold>
    {
        public SoldUnitsByDateSpecification(DateTime? start, DateTime? end)
        {
            Query.IgnoreQueryFilters()
                 .Where(x => x.ProductId != null)
                 .AsNoTracking();

            if (start != null)
            {
                Query.Where(x => x.Invoice!.CreationDate >= start);
            }
            if (end != null)
            {
                Query.Where(x => x.Invoice!.CreationDate <= end);
            }
        }
    }
}
