using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FinancingPlanSpecification
{
    public class FilterAndPaginationFinancingPlanSpecification : Specification<FinancingPlan>
    {
        public FilterAndPaginationFinancingPlanSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x => x.Provider).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) || x.Provider!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name : column == "Provider" ? x.Provider!.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name : column == "Provider" ? x.Provider!.Name : null);
                }
            }
        }
    }
}
