using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoicesInMonthAndYearSpecification : Specification<Invoice>
    {
        public FilterInvoicesInMonthAndYearSpecification(int month, int year, Guid? userId, int? branchId) {
            Query.Include(x => x.ProductsSold!).ThenInclude(x => x.Product)
                .Include(x => x.User)
                .Where(x=> x.CreationDate.Month == month && x.CreationDate.Year == year).AsNoTracking();
            if (userId != null)
            {
                Query.Where(x => x.UserId == userId);
            }
            if (branchId != null)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchId);
            }
        }
    }
}
