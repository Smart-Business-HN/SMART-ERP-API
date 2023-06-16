using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpinionSpecification
{
    public class FilterAndPaginationOpinionSpecification : Specification<Opinion>
    {
        public FilterAndPaginationOpinionSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Title.Contains(parameter)
                || x.Customer!.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Title" ? x.Title
                    : column == "Customer" ? x.Customer : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Title" ? x.Title
                    : column == "Customer" ? x.Customer : null);
                }
            }
        }
    }
}
