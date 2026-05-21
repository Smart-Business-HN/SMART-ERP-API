using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PriceListSpecification
{
    public class PriceListItemsByListSpecification : Specification<PriceListItem>
    {
        public PriceListItemsByListSpecification(int priceListId, string? parameter,
            int pageNumber, int pageSize)
        {
            Query.Where(x => x.PriceListId == priceListId)
                 .Include(x => x.Product!)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Product!.Name.Contains(parameter) || x.Product.Code.Contains(parameter));
            }

            Query.OrderBy(x => x.Product!.Name)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize);
        }
    }

    public class CountPriceListItemsByListSpecification : Specification<PriceListItem>
    {
        public CountPriceListItemsByListSpecification(int priceListId, string? parameter)
        {
            Query.Where(x => x.PriceListId == priceListId).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Product!.Name.Contains(parameter) || x.Product.Code.Contains(parameter));
            }
        }
    }

    public class PriceListItemByListAndProductSpecification : Specification<PriceListItem>
    {
        public PriceListItemByListAndProductSpecification(int priceListId, int productId)
        {
            Query.Where(x => x.PriceListId == priceListId && x.ProductId == productId);
        }
    }

    public class PriceListItemsByProductSpecification : Specification<PriceListItem>
    {
        public PriceListItemsByProductSpecification(int productId)
        {
            Query.Where(x => x.ProductId == productId).AsNoTracking();
        }
    }
}
