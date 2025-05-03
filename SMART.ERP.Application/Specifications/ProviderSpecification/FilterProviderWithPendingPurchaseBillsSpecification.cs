using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System.Linq.Expressions;

namespace SMART.ERP.Application.Specifications.ProviderSpecification;

public sealed class FilterProviderWithPendingPurchaseBillsSpecification : Specification<Provider>
{
    public FilterProviderWithPendingPurchaseBillsSpecification(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        string? orderBy,
        string? sortDirection)
    {
        Query.Include(x => x.PurchaseBills!
                .Where(pb => pb.Outstanding > 0))
            .Include(x => x.NonBillableExpenses!
                .Where(nbe => nbe.Outstanding > 0))
            .AsSplitQuery();

        Query.Where(x => x.PurchaseBills!.Any(pb => pb.Outstanding > 0) || x.NonBillableExpenses!.Any(nbe => nbe.Outstanding > 0));
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Query.Search(x => x.Name, $"%{searchTerm}%");
        }
        if (pageSize > 0)
        {
            Query.Skip((pageNumber) * pageSize).Take(pageSize);
        }
        ApplyOrdering(orderBy, sortDirection);
        Query.AsNoTracking();
    }

    private void ApplyOrdering(string? orderBy, string? sortDirection)
    {
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            var isAscending = sortDirection?.ToLower() == "asc";
            
            Expression<Func<Provider, object>> orderExpression = orderBy.ToLower() switch
            {
                "name" => x => x.Name,
                "rtn" => x => x.RTN,
                "creationdate" => x => x.CreationDate,
                _ => x => x.Id
            };

            if (isAscending)
            {
                Query.OrderBy(orderExpression);
            }
            else
            {
                Query.OrderByDescending(orderExpression);
            }
        }
        else
        {
            Query.OrderByDescending(x => x.Id);
        }
    }
}