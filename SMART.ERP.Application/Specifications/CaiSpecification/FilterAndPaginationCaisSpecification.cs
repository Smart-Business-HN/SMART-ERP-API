using Ardalis.Specification;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Specifications.CaiSpecification
{
    public class FilterAndPaginationCaisSpecification : Specification<Cai>
    {
      public FilterAndPaginationCaisSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter));
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
