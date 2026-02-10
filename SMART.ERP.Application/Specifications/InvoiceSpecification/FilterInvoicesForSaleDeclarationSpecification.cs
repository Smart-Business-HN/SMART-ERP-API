using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoicesForSaleDeclarationSpecification : Specification<Invoice>
    {
        public FilterInvoicesForSaleDeclarationSpecification(int month, int year)
        {
            Query
                .Include(x => x.Customer)
                .Include(x => x.Cai)
                .Where(x => x.CreationDate.Month == month && x.CreationDate.Year == year && x.StatusId != 17)
                .AsNoTracking();
        }
    }
}
