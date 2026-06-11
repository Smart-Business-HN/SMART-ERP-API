using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoicesInYearByUserSpecification : Specification<Invoice>
    {
        public FilterInvoicesInYearByUserSpecification(int year, Guid? id) {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product)
                    .ThenInclude(x => x!.SubCategory)
                        .Where(x=> x.CreationDate.Year == year).AsNoTracking();
            if (id != null)
            {
                Query.Where(x => x.UserId == id);
            }
        }
    }
}
