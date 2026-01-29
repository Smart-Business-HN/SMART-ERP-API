using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public sealed class FilterInvoiceByYearSpecification : Specification<Invoice>
    {
        public FilterInvoiceByYearSpecification(DateTime date) {
            Query.Include(x=>x.ProductsSold!).ThenInclude(x=>x.Product!).ThenInclude(x=>x.Brand)!
                 .Include(x=>x.ProductsSold)!.ThenInclude(x=>x.Product).ThenInclude(x=>x!.SubCategory).ThenInclude(x=>x!.Category)
                 .Where(x => x.CreationDate.Year == date.Year);
        }
    }
}
