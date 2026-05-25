using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSpecification
{
    /// <summary>
    /// Carga la cotización para visualización (detalle admin + PDF) incluyendo los componentes
    /// del combo y su UnitOfMeasurement. Usa AsNoTrackingWithIdentityResolution para evitar
    /// que UoM compartidos entre varios componentes generen instancias duplicadas que rompen
    /// el change tracker si la entidad se Update-a después en el mismo DbContext.
    /// </summary>
    public class GetQuotationDetailWithComboComponentsSpecification : Specification<Quotation>
    {
        public GetQuotationDetailWithComboComponentsSpecification(int id)
        {
            Query.Include(x => x.Status)
                 .Include(x => x.Customer).ThenInclude(x => x!.DeliveryDirections)!.ThenInclude(x => x.City).ThenInclude(x => x!.Department)
                 .Include(x => x.Prefix).ThenInclude(x => x!.InternalDocument)
                 .Include(x => x.User)
                 .Include(x => x.ProductsOffered)
                 .Include(x => x.ProductsOffered)!.ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
                 .Include(x => x.ProductsOffered)!.ThenInclude(x => x.Product!).ThenInclude(p => p.Components!).ThenInclude(c => c.Product!).ThenInclude(cp => cp!.UnitOfMeasurement)
                 .Include(x => x.ProductsOffered)!.ThenInclude(x => x.Tax)
                 .Include(x => x.BranchOffice)
                 .Where(x => x.Id == id)
                 .AsNoTrackingWithIdentityResolution();
        }
    }
}
