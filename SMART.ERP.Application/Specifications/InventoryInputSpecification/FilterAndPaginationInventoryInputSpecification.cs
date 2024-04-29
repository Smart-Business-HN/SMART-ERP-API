using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryInputSpecification
{
    public class FilterAndPaginationInventoryInputSpecification : Specification<InventoryInput>
    {
        public FilterAndPaginationInventoryInputSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x => x.Warehouse).Include(x => x.Status).Include(x => x.InventoryInputType).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Description.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Description" ? x.Description : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Description" ? x.Description : null);
                }
            }
        }
    }
}
