using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceInYearByBranchSpecification : Specification<Invoice>
    {
        public FilterInvoiceInYearByBranchSpecification(int year, int branchId) {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.ProductsSold!)
               .ThenInclude(x => x.Product).Where(x =>  x.CreationDate.Year == year && x.User!.BranchOfficeId == branchId).AsNoTracking();
        }
    }
}
