using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.LedgerAccountSpecification
{
    public class FilterAndPaginationLedgerAccountSpecification : Specification<LedgerAccount>
    {
        public FilterAndPaginationLedgerAccountSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.Parent)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .OrderBy(x => x.Code)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Code.Contains(parameter) || x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name : x.Code);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name : x.Code);
                }
            }
        }
    }
}
