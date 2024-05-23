using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByIdSpecification : Specification<Invoice>
    {
        public FilterInvoiceByIdSpecification(int id)
        {
            Query.Include(x => x.Status)
           .Include(x => x.Customer).ThenInclude(x => x!.DeliveryDirections)!.ThenInclude(x => x.City).ThenInclude(x => x.Department)
           .Include(x => x.Cai)
           .Include(x => x.User)
           .Include(x => x.ProductsSold)!.ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
           .Include(x => x.ProductsSold)!.ThenInclude(x => x.Tax)
           .Include(x => x.BranchOffice)
           .Include(x => x.BillPayments)!.ThenInclude(x => x.TypeOfPaymentMethod)
           .Include(x => x.InvoicePaymentType)
           .Where(x => x.Id == id).AsNoTracking();
        }
    }
}
