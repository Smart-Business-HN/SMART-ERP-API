using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetAccountsPayableAgingQuery : IRequest<Response<AccountsPayableAgingDto>>
    {
    }

    public class GetAccountsPayableAgingQueryHandler : IRequestHandler<GetAccountsPayableAgingQuery, Response<AccountsPayableAgingDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepository;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepository;

        public GetAccountsPayableAgingQueryHandler(
            IRepositoryAsync<PurchaseBill> purchaseBillRepository,
            IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepository)
        {
            _purchaseBillRepository = purchaseBillRepository;
            _nonBillableExpenseRepository = nonBillableExpenseRepository;
        }

        public async Task<Response<AccountsPayableAgingDto>> Handle(GetAccountsPayableAgingQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var pendingBills = await _purchaseBillRepository.ListAsync(new FilterPurchaseBillByPendingWithProviderSpecification(), cancellationToken);
            var pendingExpenses = await _nonBillableExpenseRepository.ListAsync(new FilterNonBillableExpenseByPendingWithProviderSpecification(), cancellationToken);

            var result = new AccountsPayableAgingDto();

            foreach (var bill in pendingBills)
            {
                var daysOverdue = (today - bill.InvoiceDate).Days;
                var bucket = GetAgingBucket(daysOverdue);

                switch (bucket)
                {
                    case "0-30": result.Current += bill.Outstanding; break;
                    case "31-60": result.Days31To60 += bill.Outstanding; break;
                    case "61-90": result.Days61To90 += bill.Outstanding; break;
                    default: result.Over90Days += bill.Outstanding; break;
                }

                result.Details.Add(new AgingDetailDto
                {
                    EntityId = bill.ProviderId.ToString(),
                    EntityName = bill.Provider?.Name ?? "Desconocido",
                    Outstanding = bill.Outstanding,
                    DaysOverdue = Math.Max(0, daysOverdue),
                    AgingBucket = bucket
                });
            }

            foreach (var expense in pendingExpenses)
            {
                var daysOverdue = (today - expense.Date).Days;
                var bucket = GetAgingBucket(daysOverdue);

                switch (bucket)
                {
                    case "0-30": result.Current += expense.Outstanding; break;
                    case "31-60": result.Days31To60 += expense.Outstanding; break;
                    case "61-90": result.Days61To90 += expense.Outstanding; break;
                    default: result.Over90Days += expense.Outstanding; break;
                }

                result.Details.Add(new AgingDetailDto
                {
                    EntityId = expense.ProviderId.ToString(),
                    EntityName = expense.Provider?.Name ?? "Gasto sin proveedor",
                    Outstanding = expense.Outstanding,
                    DaysOverdue = Math.Max(0, daysOverdue),
                    AgingBucket = bucket
                });
            }

            result.Total = result.Current + result.Days31To60 + result.Days61To90 + result.Over90Days;
            return new Response<AccountsPayableAgingDto>(result);
        }

        private static string GetAgingBucket(int daysOverdue)
        {
            return daysOverdue switch
            {
                <= 30 => "0-30",
                <= 60 => "31-60",
                <= 90 => "61-90",
                _ => "90+"
            };
        }
    }
}
