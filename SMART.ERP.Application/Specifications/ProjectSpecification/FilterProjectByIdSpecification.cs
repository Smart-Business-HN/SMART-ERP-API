using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProjectSpecification
{
    public class FilterProjectByIdSpecification : Specification<Project>
    {
        public FilterProjectByIdSpecification(int id)
        {
            Query
                .Include(x => x.Customer)
                .Include(x => x.Status)
                .Include(x => x.Prefix)
                .Include(x => x.PurchaseBills)!.ThenInclude(x => x.Provider)
                .Include(x => x.NonBillableExpenses)!.ThenInclude(x => x.Provider)
                .Include(x => x.Invoices)!.ThenInclude(x => x.Customer)
                .Include(x => x.Quotations)!.ThenInclude(x => x.Customer)
                .Include(x => x.InventoryExits)!.ThenInclude(x => x.Items)
                .Include(x => x.InventoryEntries)!.ThenInclude(x => x.Items)
                .Where(x => x.Id == id);
        }
    }
}
