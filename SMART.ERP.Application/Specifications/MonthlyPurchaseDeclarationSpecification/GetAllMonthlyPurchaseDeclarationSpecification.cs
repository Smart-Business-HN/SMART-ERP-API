
using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MonthlyPurchaseDeclarationSpecification
{
    public class GetAllMonthlyPurchaseDeclarationSpecification : Specification<MonthlyPurchaseDeclaration>
    {
        public GetAllMonthlyPurchaseDeclarationSpecification(string? parameter, int pageNumber, int pageSize, string? order, string? column)
        {
            Query
                .Include(x => x.Status)
                .Include(x => x.DeclaratedPurchaseBills)
                .Skip(pageNumber * pageSize).Take(pageSize).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Period.Contains(parameter));
            }
        }
    }
}
