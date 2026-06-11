using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    /// <summary>
    /// Carga la factura para visualización (detalle admin + PDF) incluyendo los componentes
    /// del combo y su UnitOfMeasurement. Usa AsNoTrackingWithIdentityResolution para evitar
    /// que UoM compartidos entre varios componentes generen instancias duplicadas que rompen
    /// el change tracker si la entidad se Update-a después en el mismo DbContext.
    /// </summary>
    public class GetInvoiceDetailWithComboComponentsSpecification : Specification<Invoice>
    {
        public GetInvoiceDetailWithComboComponentsSpecification(int id)
        {
            // IgnoreQueryFilters: la factura debe seguir mostrando productos eliminados (historico).
            Query.IgnoreQueryFilters()
                 .Include(x => x.Status)
                 .Include(x => x.Customer).ThenInclude(x => x!.DeliveryDirections)!.ThenInclude(x => x.City).ThenInclude(x => x!.Department)
                 .Include(x => x.Cai)
                 .Include(x => x.User)
                 .Include(x => x.ProductsSold)!.ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
                 .Include(x => x.ProductsSold)!.ThenInclude(x => x.Product!).ThenInclude(p => p.Components!).ThenInclude(c => c.Product!).ThenInclude(cp => cp!.UnitOfMeasurement)
                 .Include(x => x.ProductsSold)!.ThenInclude(x => x.Tax)
                 .Include(x => x.BranchOffice)
                 .Include(x => x.BillPayments)!.ThenInclude(x => x.TypeOfPaymentMethod)
                 .Include(x => x.InvoicePaymentType)
                 .Where(x => x.Id == id)
                 .AsNoTrackingWithIdentityResolution();
        }
    }
}
