using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ReportSpecification
{
    public class ClientBySoldProductSpecification : Specification<Invoice>
    {
        public ClientBySoldProductSpecification(int productId)
        {
            Query.Include(x => x.Customer).Include(x => x.ProductsSold.Where(y => y.ProductId == productId))
                .Include(x => x.User).Where(x => x.ProductsSold.Any(y => y.ProductId == productId));
        }
    }
}
