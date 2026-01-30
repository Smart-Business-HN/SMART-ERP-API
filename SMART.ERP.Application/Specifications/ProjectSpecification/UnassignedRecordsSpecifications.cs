using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProjectSpecification
{
    // Invoice - Paginated
    public class FilterUnassignedInvoicesSpecification : Specification<Invoice>
    {
        public FilterUnassignedInvoicesSpecification(string? parameter, int pageNumber, int pageSize)
        {
            Query.Where(x => x.ProjectId == null)
                 .Include(x => x.Customer)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .OrderByDescending(x => x.Id)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.InvoiceNumber != null && x.InvoiceNumber.Contains(parameter)) ||
                                 (x.Customer != null && x.Customer.FullName.Contains(parameter)));
            }
        }
    }

    // Invoice - Count
    public class CountUnassignedInvoicesSpecification : Specification<Invoice>
    {
        public CountUnassignedInvoicesSpecification(string? parameter)
        {
            Query.Where(x => x.ProjectId == null);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.InvoiceNumber != null && x.InvoiceNumber.Contains(parameter)) ||
                                 (x.Customer != null && x.Customer.FullName.Contains(parameter)));
            }
        }
    }

    // PurchaseBill - Paginated
    public class FilterUnassignedPurchaseBillsSpecification : Specification<PurchaseBill>
    {
        public FilterUnassignedPurchaseBillsSpecification(string? parameter, int pageNumber, int pageSize)
        {
            Query.Where(x => x.ProjectId == null)
                 .Include(x => x.Provider)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .OrderByDescending(x => x.Id)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.PurchaseBillCode != null && x.PurchaseBillCode.Contains(parameter)) ||
                                 (x.Provider != null && x.Provider.Name.Contains(parameter)));
            }
        }
    }

    // PurchaseBill - Count
    public class CountUnassignedPurchaseBillsSpecification : Specification<PurchaseBill>
    {
        public CountUnassignedPurchaseBillsSpecification(string? parameter)
        {
            Query.Where(x => x.ProjectId == null);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.PurchaseBillCode != null && x.PurchaseBillCode.Contains(parameter)) ||
                                 (x.Provider != null && x.Provider.Name.Contains(parameter)));
            }
        }
    }

    // NonBillableExpense - Paginated
    public class FilterUnassignedNonBillableExpensesSpecification : Specification<NonBillableExpense>
    {
        public FilterUnassignedNonBillableExpensesSpecification(string? parameter, int pageNumber, int pageSize)
        {
            Query.Where(x => x.ProjectId == null)
                 .Include(x => x.Provider)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .OrderByDescending(x => x.Id)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.ExpenseCode != null && x.ExpenseCode.Contains(parameter)) ||
                                 (x.Description != null && x.Description.Contains(parameter)));
            }
        }
    }

    // NonBillableExpense - Count
    public class CountUnassignedNonBillableExpensesSpecification : Specification<NonBillableExpense>
    {
        public CountUnassignedNonBillableExpensesSpecification(string? parameter)
        {
            Query.Where(x => x.ProjectId == null);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.ExpenseCode != null && x.ExpenseCode.Contains(parameter)) ||
                                 (x.Description != null && x.Description.Contains(parameter)));
            }
        }
    }

    // Quotation - Paginated
    public class FilterUnassignedQuotationsSpecification : Specification<Quotation>
    {
        public FilterUnassignedQuotationsSpecification(string? parameter, int pageNumber, int pageSize)
        {
            Query.Where(x => x.ProjectId == null)
                 .Include(x => x.Customer)
                 .Skip(pageNumber * pageSize)
                 .Take(pageSize)
                 .OrderByDescending(x => x.Id)
                 .AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.QuotationCode != null && x.QuotationCode.Contains(parameter)) ||
                                 (x.Customer != null && x.Customer.FullName.Contains(parameter)));
            }
        }
    }

    // Quotation - Count
    public class CountUnassignedQuotationsSpecification : Specification<Quotation>
    {
        public CountUnassignedQuotationsSpecification(string? parameter)
        {
            Query.Where(x => x.ProjectId == null);

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => (x.QuotationCode != null && x.QuotationCode.Contains(parameter)) ||
                                 (x.Customer != null && x.Customer.FullName.Contains(parameter)));
            }
        }
    }
}
