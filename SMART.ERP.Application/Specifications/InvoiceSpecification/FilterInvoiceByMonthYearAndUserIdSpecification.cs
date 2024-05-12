using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByMonthYearAndUserIdSpecification : Specification<Invoice>
    {
        public FilterInvoiceByMonthYearAndUserIdSpecification(int month, int year, Guid userId, int? branchId)
        {
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product).Include(x => x.User)
                .Where(x => x.CreationDate.Month == month && x.CreationDate.Year == year && x.UserId == userId).AsNoTracking();

            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
