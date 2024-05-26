using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MonthlyPurchaseDeclarationSpecification
{
    public class GetMonthlyPurchaseDeclarationByIdSpecification : Specification<MonthlyPurchaseDeclaration>
    {
        public GetMonthlyPurchaseDeclarationByIdSpecification(int id)
        {
            Query.Include(x => x.Status).Include(x => x.DeclaratedPurchaseBills).Where(x => x.Id == id);
        }
    }
}
