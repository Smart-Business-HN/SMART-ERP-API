using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GeneralFinanceQuery : IRequest<Response<GeneralFinanceInformationDto>>
    {
    }
    public class GeneralFinanceQueryHandler : IRequestHandler<GeneralFinanceQuery, Response<GeneralFinanceInformationDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        public GeneralFinanceQueryHandler(IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync)
        {
            _purchaseBillRepositoryAsync = purchaseBillRepositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
        }
        public async Task<Response<GeneralFinanceInformationDto>> Handle(GeneralFinanceQuery request, CancellationToken cancellationToken)
        {
            DateTime currentDate = DateTime.Now;
            List<string> brands = new List<string> {};
            List<decimal> brandvalues = new List<decimal> {};
            List<string> expenses = new List<string> {};
            List<decimal> expensesValues = new List<decimal> {};
            var bills = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByYearSpecification(currentDate));
            var billsOfThisMonth = bills.FindAll(x=>x.InvoiceDate.Month == currentDate.Month);
            var billOfLastMonth = bills.FindAll(x=>x.InvoiceDate.Month == currentDate.Month - 1);
            var invoices = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByYearSpecification(currentDate));
            var invoicesOfThisMonth = invoices.FindAll(x => x.CreationDate.Month == currentDate.Month);
            var invoicesOfLastMonth = invoices.FindAll(x => x.CreationDate.Month == currentDate.Month - 1);
            bills.ForEach(bill =>
            {
                bool exist = expenses.Contains(bill.ExpenseAccount.Name);
                if(!exist)
                {
                    expenses.Add(bill.ExpenseAccount.Name);
                }
            });
            expenses.ForEach(expense =>
            {
                decimal value = 0;
                bills.ForEach(bill =>
                {
                    if(bill.ExpenseAccount.Name == expense)
                    {
                        value += bill.Total;
                    }
                });
                expensesValues.Add(value);
            });
            invoices.ForEach(invoice => {
                invoice.ProductsSold.ForEach(product =>
                {
                    bool exist = brands.Contains(product.Product.Brand.Name);
                    if(!exist)
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
                        if(product.Product.Brand.Name == brand)
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
                AnnualExpenses = bills.Sum(x => x.Total),
                CurrentMonthExpenses = billsOfThisMonth.Sum(x => x.Total),
                CurrentMonthSales = invoicesOfThisMonth.Sum(x => x.Total),
                PreviousMonthExpenses = billOfLastMonth.Sum(x => x.Total),
                PreviousMonthSales = invoicesOfLastMonth.Sum(x => x.Total),
                BrandSales = brandsSales,
                Expenses= expenseValues
            };
            return new Response<GeneralFinanceInformationDto>(values);

        }
    }
}
