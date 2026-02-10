using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MonthlySaleDeclarationSpecification
{
    public class GetAllMonthlySaleDeclarationSpecification : Specification<MonthlySaleDeclaration>
    {
        public GetAllMonthlySaleDeclarationSpecification(string? parameter, int pageNumber, int pageSize, string? order, string? column)
        {
            Query
                .Include(x => x.Status)
                .Include(x => x.DeclaredSaleInvoices)
                .Skip(pageNumber * pageSize).Take(pageSize).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Period.Contains(parameter));
            }
        }
    }
}
