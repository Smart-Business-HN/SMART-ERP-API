using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.NonBillableExpenseSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetTopSuppliersOutstandingQuery : IRequest<Response<TopSuppliersOutstandingDto>>
    {
        public int Top { get; set; } = 5;
    }

    public class GetTopSuppliersOutstandingQueryHandler : IRequestHandler<GetTopSuppliersOutstandingQuery, Response<TopSuppliersOutstandingDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepository;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepository;

        public GetTopSuppliersOutstandingQueryHandler(
            IRepositoryAsync<PurchaseBill> purchaseBillRepository,
            IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepository)
        {
            _purchaseBillRepository = purchaseBillRepository;
            _nonBillableExpenseRepository = nonBillableExpenseRepository;
        }

        public async Task<Response<TopSuppliersOutstandingDto>> Handle(GetTopSuppliersOutstandingQuery request, CancellationToken cancellationToken)
        {
            var pendingBills = await _purchaseBillRepository.ListAsync(
                new FilterPurchaseBillByPendingWithProviderSpecification(), cancellationToken);
            var pendingExpenses = await _nonBillableExpenseRepository.ListAsync(
                new FilterNonBillableExpenseByPendingWithProviderSpecification(), cancellationToken);

            var billGroups = pendingBills
                .GroupBy(b => new { b.ProviderId, ProviderName = b.Provider?.Name ?? "Desconocido" })
                .Select(g => new
                {
                    g.Key.ProviderId,
                    g.Key.ProviderName,
                    Outstanding = g.Sum(b => b.Outstanding),
                    Count = g.Count(),
                    OldestDate = g.Min(b => b.InvoiceDate)
                });

            var expenseGroups = pendingExpenses
                .GroupBy(e => new { e.ProviderId, ProviderName = e.Provider?.Name ?? "Gasto sin proveedor" })
                .Select(g => new
                {
                    g.Key.ProviderId,
                    g.Key.ProviderName,
                    Outstanding = g.Sum(e => e.Outstanding),
                    Count = g.Count(),
                    OldestDate = g.Min(e => e.Date)
                });

            var combined = billGroups
                .Concat(expenseGroups)
                .GroupBy(x => new { x.ProviderId, x.ProviderName })
                .Select(g => new SupplierOutstandingDto
                {
                    ProviderId = g.Key.ProviderId,
                    ProviderName = g.Key.ProviderName,
                    TotalOutstanding = g.Sum(x => x.Outstanding),
                    BillCount = g.Sum(x => x.Count),
                    OldestBillDate = g.Min(x => x.OldestDate)
                })
                .OrderByDescending(s => s.TotalOutstanding)
                .Take(request.Top)
                .ToList();

            return new Response<TopSuppliersOutstandingDto>(new TopSuppliersOutstandingDto { Suppliers = combined });
        }
    }
}
