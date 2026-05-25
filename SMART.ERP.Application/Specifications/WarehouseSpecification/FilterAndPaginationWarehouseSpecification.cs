using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.WarehouseSpecification
{
    public class FilterAndPaginationWarehouseSpecification : Specification<Warehouse>
    {
        public FilterAndPaginationWarehouseSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column, int? branchOfficeId = null)
        {
            Query
                .Include(x => x.BranchOffice)
                .Include(x => x.City)
                .Include(x => x.User)
                .Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter));
            }

            if (branchOfficeId.HasValue)
            {
                Query.Where(x => x.BranchOfficeId == branchOfficeId.Value);
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name : null);
                }
            }
        }
    }
}
