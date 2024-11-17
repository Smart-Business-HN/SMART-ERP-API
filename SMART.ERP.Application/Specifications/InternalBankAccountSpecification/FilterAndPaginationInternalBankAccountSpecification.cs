using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InternalBankAccountSpecification
{
    public class FilterAndPaginationInternalBankAccountSpecification : Specification<InternalBankAccount>
    {
        public FilterAndPaginationInternalBankAccountSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column) 
        {
            Query.Include(x => x.Bank).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Include(x=>x.Bank).Where(x => x.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.Include(x=>x.Bank).OrderByDescending(x => column == "Name" ? x.Name : column == "DepartmentId" ? x.AccountNumber : column == "IsActive" ? x.Bank!.Name : null);
                }
                else
                {
                    Query.Include(x => x.Bank).OrderBy(x => column == "Name" ? x.Name : column == "DepartmentId" ? x.AccountNumber : column == "IsActive" ? x.Bank!.Name : null);
                }
            }
        }
    }
}
