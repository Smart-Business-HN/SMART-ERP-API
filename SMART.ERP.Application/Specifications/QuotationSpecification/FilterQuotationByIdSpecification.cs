using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSpecification
{
    public class FilterQuotationByIdSpecification : Specification<Quotation>
    {
        public FilterQuotationByIdSpecification(int id)
        {
           // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
           Query.IgnoreQueryFilters();
           Query.Include(x => x.Status)
                .Include(x => x.Customer).ThenInclude(x => x!.DeliveryDirections)!.ThenInclude(x => x.City).ThenInclude(x => x!.Department)
                .Include(x => x.Prefix).ThenInclude(x => x!.InternalDocument)
                .Include(x => x.User)
                .Include(x => x.ProductsOffered)
                .Include(x => x.ProductsOffered)!.ThenInclude(x=>x.Product).ThenInclude(x=>x!.Brand)
                .Include(x => x.ProductsOffered)!.ThenInclude(x=>x.Tax)
                .Include(x => x.BranchOffice)
                .Where(x => x.Id == id).AsNoTracking();
        }
    }
}
