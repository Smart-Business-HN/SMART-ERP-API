using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetFinancialKpisQuery : IRequest<Response<FinancialKpisDto>>
    {
    }

    public class GetFinancialKpisQueryHandler : IRequestHandler<GetFinancialKpisQuery, Response<FinancialKpisDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepository;
        private readonly IRepositoryAsync<InternalBankAccount> _bankAccountRepository;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepository;

        public GetFinancialKpisQueryHandler(
            IRepositoryAsync<Invoice> invoiceRepository,
            IRepositoryAsync<PurchaseBill> purchaseBillRepository,
            IRepositoryAsync<InternalBankAccount> bankAccountRepository,
            IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepository)
        {
            _invoiceRepository = invoiceRepository;
            _purchaseBillRepository = purchaseBillRepository;
            _bankAccountRepository = bankAccountRepository;
            _nonBillableExpenseRepository = nonBillableExpenseRepository;
        }

        public async Task<Response<FinancialKpisDto>> Handle(GetFinancialKpisQuery request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.Now;
            var yearStart = new DateTime(currentDate.Year, 1, 1);

            var pendingInvoices = await _invoiceRepository.ListAsync(new FilterInvoiceByPendingValuesSpecification(), cancellationToken);
            var pendingBills = await _purchaseBillRepository.ListAsync(new FilterPurchaseBillByPendingValuesSpecification(), cancellationToken);
            var pendingNonBillable = await _nonBillableExpenseRepository.ListAsync(new FilterNonBillableExpenseByPendingValuesSpecification(), cancellationToken);
            var bankAccounts = await _bankAccountRepository.ListAsync(cancellationToken);
            var yearInvoices = await _invoiceRepository.ListAsync(new FilterInvoiceByYearSpecification(currentDate), cancellationToken);
            var yearBills = await _purchaseBillRepository.ListAsync(new FilterPurchaseBillByYearSpecification(currentDate), cancellationToken);
            var yearNonBillable = await _nonBillableExpenseRepository.ListAsync(new FilterNonBillableExpenseByYearSpecification(currentDate), cancellationToken);

            decimal accountsReceivable = pendingInvoices.Sum(i => i.Outstanding);
            decimal accountsPayable = pendingBills.Sum(b => b.Outstanding) + pendingNonBillable.Sum(n => n.Outstanding);
            decimal cashBalance = bankAccounts.Where(b => b.AccountType != InternalBankAccountType.CreditCard).Sum(b => b.CurrentAmount);
            decimal creditCardPayable = bankAccounts.Where(b => b.AccountType == InternalBankAccountType.CreditCard).Sum(b => b.CurrentAmount);
            decimal annualSales = yearInvoices.Sum(i => i.Total);
            decimal annualCOGS = yearBills.Sum(b => b.Total) + yearNonBillable.Sum(n => n.Amount);

            decimal currentAssets = cashBalance + accountsReceivable;
            decimal totalLiabilities = accountsPayable + creditCardPayable;
            decimal currentLiabilities = totalLiabilities > 0 ? totalLiabilities : 1;

            decimal dso = annualSales > 0 ? (accountsReceivable / annualSales) * 365 : 0;
            decimal dpo = annualCOGS > 0 ? (totalLiabilities / annualCOGS) * 365 : 0;

            var result = new FinancialKpisDto
            {
                AcidTest = (cashBalance + accountsReceivable) / currentLiabilities,
                CurrentRatio = currentAssets / currentLiabilities,
                WorkingCapital = currentAssets - totalLiabilities,
                CashBalance = cashBalance,
                DaysSalesOutstanding = Math.Round(dso, 1),
                DaysPayableOutstanding = Math.Round(dpo, 1),
                CashConversionCycle = Math.Round(dso - dpo, 1),
                GrossProfitMargin = annualSales > 0 ? Math.Round(((annualSales - annualCOGS) / annualSales) * 100, 2) : 0,
                Receivable = accountsReceivable,
                Payable = accountsPayable,
                CreditCardPayable = creditCardPayable
            };

            return new Response<FinancialKpisDto>(result);
        }
    }
}
