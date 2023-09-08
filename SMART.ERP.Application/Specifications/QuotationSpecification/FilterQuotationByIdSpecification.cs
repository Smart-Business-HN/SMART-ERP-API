using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.QuotationSpecification
{
    public class FilterQuotationByIdSpecification : Specification<Quotation>
    {
        public FilterQuotationByIdSpecification(int id)
        {
            Query
                .Include(x=>x.Status)
                .Include(x=>x.Customer)
                .Include(x=>x.Prefix)
                .Include(x=>x.User)
                .Include(x=>x.ProductsOffered)!.ThenInclude(x=>x.Product).ThenInclude(x=>x!.Brand)
                .Include(x=>x.ProductsOffered)!.ThenInclude(x=>x.Tax)
                .Include(x=>x.BranchOffice)
                .Where(x => x.Id == id).AsNoTracking();
        }
    }
}
