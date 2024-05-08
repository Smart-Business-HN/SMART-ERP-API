using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.DailyCloseSpecification
{
    public class FilterAndPaginationDailyClosingSpecification : Specification<DailyClose>
    {
        public FilterAndPaginationDailyClosingSpecification(string? parameter, int pageNumber,
                       int pageSize, string? order, string? column)
        {
            Query.Include(x => x.BranchOffice).Include(x => x.Cai).Include(x => x.ResumePayments).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.BranchOffice.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "BranchOffice" ? x.BranchOffice.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Cai" ? x.Cai.Identificator : null);
                }
            }
        }
    }
}
