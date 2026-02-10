using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MonthlySaleDeclarationSpecification
{
    public class GetMonthlySaleDeclarationByIdSpecification : Specification<MonthlySaleDeclaration>
    {
        public GetMonthlySaleDeclarationByIdSpecification(int id)
        {
            Query.Include(x => x.Status).Include(x => x.DeclaredSaleInvoices).Where(x => x.Id == id);
        }
    }
}
