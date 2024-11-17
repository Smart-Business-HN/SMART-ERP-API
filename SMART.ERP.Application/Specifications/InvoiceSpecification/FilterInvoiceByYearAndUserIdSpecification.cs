using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByYearAndUserIdSpecification : Specification<Invoice>
    {
        public FilterInvoiceByYearAndUserIdSpecification(int year, Guid id)
        {
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product)
            .Where(x => x.CreationDate.Year == year && x.UserId == id && x.Status!.Name != "Cancelada").AsNoTracking();
        }
    }
}
