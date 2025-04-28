using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.InventoryDistributionSpecification;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GeneralFinanceQuery : IRequest<Response<GeneralFinanceInformationDto>>
    {
    }
    public class GeneralFinanceQueryHandler : IRequestHandler<GeneralFinanceQuery, Response<GeneralFinanceInformationDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<InternalBankAccount> _internalBankAccountRepositoryAsync;
        private readonly IRepositoryAsync<AdvisorGoal> _advisorGoalRepositoryAsync;
        private readonly IRepositoryAsync<InventoryDistribution> _inventoryDistributionRepositoryAsync;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepositoryAsync;
        public GeneralFinanceQueryHandler(IRepositoryAsync<InventoryDistribution> inventoryDistributionRepositoryAsync, IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync, IRepositoryAsync<AdvisorGoal> advisorGoalRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync, IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepositoryAsync)
        {
            _nonBillableExpenseRepositoryAsync = nonBillableExpenseRepositoryAsync;
            _inventoryDistributionRepositoryAsync = inventoryDistributionRepositoryAsync;
            _purchaseBillRepositoryAsync = purchaseBillRepositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _internalBankAccountRepositoryAsync = internalBankAccountRepositoryAsync;
            _advisorGoalRepositoryAsync = advisorGoalRepositoryAsync;
        }
        public async Task<Response<GeneralFinanceInformationDto>> Handle(GeneralFinanceQuery request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;
            List<string> brands = new List<string> { };
            List<decimal> brandvalues = new List<decimal> { };
            List<string> expenses = new List<string> { };
            List<decimal> expensesValues = new List<decimal> { };

            var purchaseBillsPendingToPay = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByPendingValuesSpecification(),cancellationToken);
            var nonBillableExpensePendingToPay = await _nonBillableExpenseRepositoryAsync.ListAsync(new FilterNonBillableExpenseByPendingValuesSpecification(),cancellationToken);
            var invoicesPendindToPay = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByPendingValuesSpecification(),cancellationToken);
            var bills = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByYearSpecification(currentDate),cancellationToken);
            var nonBillableExpenses = await _nonBillableExpenseRepositoryAsync.ListAsync(new FilterNonBillableExpenseByYearSpecification(currentDate),cancellationToken);
            var invoices = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByYearSpecification(currentDate),cancellationToken);
            var bankAccounts = await _internalBankAccountRepositoryAsync.ListAsync(cancellationToken);
            var globalGoal = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(currentDate.Year, null),cancellationToken);
            var inventoryDistributions = await _inventoryDistributionRepositoryAsync.ListAsync(new FilterInventoryDistributionByAvalibleStockSpecification(),cancellationToken);

            var billsOfThisMonth = bills.FindAll(x => x.InvoiceDate.Month == currentDate.Month);
            var nonBillableExpensesOfThisMonth = nonBillableExpenses.FindAll(x => x.Date.Month == currentDate.Month);
            var billOfLastMonth = bills.FindAll(x => x.InvoiceDate.Month == currentDate.Month - 1);
            var nonBillableExpensesOfLastMonth = nonBillableExpenses.FindAll(x => x.Date.Month == currentDate.Month - 1);
            var invoicesOfThisMonth = invoices.FindAll(x => x.CreationDate.Month == currentDate.Month);
            var invoicesOfLastMonth = invoices.FindAll(x => x.CreationDate.Month == currentDate.Month - 1);

            decimal payable = purchaseBillsPendingToPay.Sum(purchaseBill => purchaseBill.Outstanding) + nonBillableExpensePendingToPay.Sum(nonBillableExpense => nonBillableExpense.Outstanding);
            payable = payable > 0 ? payable : 1;
            decimal receivable = invoicesPendindToPay.Sum(invoice => invoice.Outstanding);
            decimal cashInBanks = bankAccounts.Sum(x => x.CurrentAmount);
            bills.ForEach(bill =>
            {
                bool exist = expenses.Contains(bill.ExpenseAccount!.Name);
                if (!exist)
                {
                    expenses.Add(bill.ExpenseAccount.Name);
                }
            });
            nonBillableExpenses.ForEach(nonBillableExpense =>
            {
                bool exist = expenses.Contains(nonBillableExpense.ExpenseAccount!.Name);
                if (!exist)
                {
                    expenses.Add(nonBillableExpense.ExpenseAccount.Name);
                }
            });
            expenses.ForEach(expense =>
            {
                decimal value = 0;
                bills.ForEach(bill =>
                {
                    if (bill.ExpenseAccount!.Name == expense)
                    {
                        value += bill.Total;
                    }
                });
                nonBillableExpenses.ForEach(nonBillableExpense =>
                {
                    if (nonBillableExpense.ExpenseAccount!.Name == expense)
                    {
                        value += nonBillableExpense.Amount;
                    }
                });
                expensesValues.Add(value);
            });
            invoices.ForEach(invoice =>
            {
                invoice.ProductsSold!.ForEach(product =>
                {
                    bool exist = brands.Contains(product.Product!.Brand!.Name);
                    if (!exist)
                    {
                        brands.Add(product.Product.Brand.Name);
                    }
                });
            });
            brands.ForEach(brand =>
            {
                decimal value = 0;
                invoices.ForEach(invoice =>
                {
                    invoice.ProductsSold!.ForEach(product =>
                    {
                        if (product.Product!.Brand!.Name == brand)
                        {
                            value += product.TotalLine;
                        }
                    });
                });
                brandvalues.Add(value);
            });
            var brandsSales = new BrandSalesDto
            {
                Brands = brands,
                Values = brandvalues
            };
            var expenseValues = new ExpensesDto
            {
                ExpenseAccounts = expenses,
                Values = expensesValues
            };
            var values = new GeneralFinanceInformationDto
            {
                AnnualSales = invoices.Sum(x => x.Total),
                AnnualGoal = globalGoal.Sum(x => x.Goal),
                AnnualExpenses = bills.Sum(x => x.Total),
                CurrentMonthExpenses = billsOfThisMonth.Sum(x => x.Total) + nonBillableExpensesOfThisMonth.Sum(x => x.Amount),
                CurrentMonthSales = invoicesOfThisMonth.Sum(x => x.Total),
                PreviousMonthExpenses = billOfLastMonth.Sum(x => x.Total) + nonBillableExpensesOfLastMonth.Sum(x => x.Amount),
                PreviousMonthSales = invoicesOfLastMonth.Sum(x => x.Total),
                BrandSales = brandsSales,
                Expenses = expenseValues,
                Payable = payable,
                Receivable = receivable,
                AcidTest = (cashInBanks + receivable) / payable,
                RealInventory = inventoryDistributions.Sum(x => x.Quantity * (x.Product!.CostPrice * (1 + (x.Product!.Tax!.Rate / 100)))),
                AccountantInventory = inventoryDistributions.Sum(x => x.Quantity * (x.Product!.RecomendedSalePrice * (1 + (x.Product!.Tax!.Rate / 100))))
            };
            return new Response<GeneralFinanceInformationDto>(values);
        }
    }
}
