using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
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
        public GeneralFinanceQueryHandler(IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync, IRepositoryAsync<AdvisorGoal> advisorGoalRepositoryAsync, IRepositoryAsync<InternalBankAccount> internalBankAccountRepositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync)
        {
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

            var purchaseBillsPendingToPay = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByPendingValuesSpecification());
            var invoicesPendindToPay = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByPendingValuesSpecification());
            var bills = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByYearSpecification(currentDate));
            var invoices = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByYearSpecification(currentDate));
            var bankAccounts = await _internalBankAccountRepositoryAsync.ListAsync();
            var globalGoal = await _advisorGoalRepositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(currentDate.Year, null));

            var billsOfThisMonth = bills.FindAll(x => x.InvoiceDate.Month == currentDate.Month);
            var billOfLastMonth = bills.FindAll(x => x.InvoiceDate.Month == currentDate.Month - 1);
            var invoicesOfThisMonth = invoices.FindAll(x => x.CreationDate.Month == currentDate.Month);
            var invoicesOfLastMonth = invoices.FindAll(x => x.CreationDate.Month == currentDate.Month - 1);

            decimal payable = purchaseBillsPendingToPay.Sum(purchaseBill => purchaseBill.Outstanding);
            payable = payable > 0 ? payable : 1;
            decimal receivable = invoicesPendindToPay.Sum(invoice => invoice.Outstanding);
            decimal cashInBanks = bankAccounts.Sum(x => x.CurrentAmount);
            bills.ForEach(bill =>
            {
                bool exist = expenses.Contains(bill.ExpenseAccount.Name);
                if (!exist)
                {
                    expenses.Add(bill.ExpenseAccount.Name);
                }
            });
            expenses.ForEach(expense =>
            {
                decimal value = 0;
                bills.ForEach(bill =>
                {
                    if (bill.ExpenseAccount.Name == expense)
                    {
                        value += bill.Total;
                    }
                });
                expensesValues.Add(value);
            });
            invoices.ForEach(invoice =>
            {
                invoice.ProductsSold.ForEach(product =>
                {
                    bool exist = brands.Contains(product.Product.Brand.Name);
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
                    invoice.ProductsSold.ForEach(product =>
                    {
                        if (product.Product.Brand.Name == brand)
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
                CurrentMonthExpenses = billsOfThisMonth.Sum(x => x.Total),
                CurrentMonthSales = invoicesOfThisMonth.Sum(x => x.Total),
                PreviousMonthExpenses = billOfLastMonth.Sum(x => x.Total),
                PreviousMonthSales = invoicesOfLastMonth.Sum(x => x.Total),
                BrandSales = brandsSales,
                Expenses = expenseValues,
                Payable = payable,
                Receivable = receivable,
                AcidTest = (cashInBanks + receivable) / payable,
            };
            return new Response<GeneralFinanceInformationDto>(values);
        }
    }
}
