using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseOrderSpecification
{
    public class FilterAndPaginationPurchaseOrderSpecification : Specification<PurchaseOrder>
    {
        public FilterAndPaginationPurchaseOrderSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x => x.ProductsToPurchase!).ThenInclude(x => x.Product).ThenInclude(x => x!.Brand)
              .Include(x => x.ProductsToPurchase!).ThenInclude(x => x.Tax)
              .Include(x => x.BranchOffice!)
              .Include(x => x.User!)
              .Include(x => x.Prefix)
              .Include(x => x.Status)
              .Include(x => x.Provider).Include(x => x.User).Skip((pageNumber) * pageSize).Take(pageSize).OrderByDescending(x => x.Id).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.PurchaseOrderCode.Contains(parameter) || x.Provider!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderBy(x => column == "QuotationCode" ? x.PurchaseOrderCode : null);
                }
                else
                {
                    Query.OrderBy(x => column == "QuotationCode" ? x.PurchaseOrderCode : null);
                }
            }
        }
    }
}
