using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BillPaymentSpecification;
using SMART.ERP.Application.Specifications.NonBillableExpensePaymentSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillPaymentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Globalization;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetMonthlyCashFlowQuery : IRequest<Response<MonthlyCashFlowDto>>
    {
        public int Months { get; set; } = 12;
    }

    public class GetMonthlyCashFlowQueryHandler : IRequestHandler<GetMonthlyCashFlowQuery, Response<MonthlyCashFlowDto>>
    {
        private readonly IRepositoryAsync<BillPayment> _billPaymentRepository;
        private readonly IRepositoryAsync<PurchaseBillPayment> _purchaseBillPaymentRepository;
        private readonly IRepositoryAsync<NonBillableExpensePayment> _nonBillablePaymentRepository;

        public GetMonthlyCashFlowQueryHandler(
            IRepositoryAsync<BillPayment> billPaymentRepository,
            IRepositoryAsync<PurchaseBillPayment> purchaseBillPaymentRepository,
            IRepositoryAsync<NonBillableExpensePayment> nonBillablePaymentRepository)
        {
            _billPaymentRepository = billPaymentRepository;
            _purchaseBillPaymentRepository = purchaseBillPaymentRepository;
            _nonBillablePaymentRepository = nonBillablePaymentRepository;
        }

        public async Task<Response<MonthlyCashFlowDto>> Handle(GetMonthlyCashFlowQuery request, CancellationToken cancellationToken)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-request.Months + 1);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);

            var inflows = await _billPaymentRepository.ListAsync(
                new FilterBillPaymentByDateRangeSpecification(startDate, endDate), cancellationToken);
            var purchaseOutflows = await _purchaseBillPaymentRepository.ListAsync(
                new FilterPurchaseBillPaymentByDateRangeSpecification(startDate, endDate), cancellationToken);
            var expenseOutflows = await _nonBillablePaymentRepository.ListAsync(
                new FilterNonBillableExpensePaymentByDateRangeSpecification(startDate, endDate), cancellationToken);

            var result = new MonthlyCashFlowDto();
            var culture = new CultureInfo("es-HN");

            for (int i = 0; i < request.Months; i++)
            {
                var monthDate = startDate.AddMonths(i);
                var monthInflows = inflows
                    .Where(p => p.Date.Year == monthDate.Year && p.Date.Month == monthDate.Month)
                    .Sum(p => p.Amount);
                var monthPurchaseOutflows = purchaseOutflows
                    .Where(p => p.Date.Year == monthDate.Year && p.Date.Month == monthDate.Month)
                    .Sum(p => p.Amount);
                var monthExpenseOutflows = expenseOutflows
                    .Where(p => p.Date.Year == monthDate.Year && p.Date.Month == monthDate.Month)
                    .Sum(p => p.Amount);
                var totalOutflows = monthPurchaseOutflows + monthExpenseOutflows;

                result.Months.Add(new CashFlowMonthDto
                {
                    Year = monthDate.Year,
                    Month = monthDate.Month,
                    MonthName = culture.DateTimeFormat.GetAbbreviatedMonthName(monthDate.Month),
                    Inflows = monthInflows,
                    Outflows = totalOutflows,
                    NetCashFlow = monthInflows - totalOutflows
                });
            }

            return new Response<MonthlyCashFlowDto>(result);
        }
    }
}
