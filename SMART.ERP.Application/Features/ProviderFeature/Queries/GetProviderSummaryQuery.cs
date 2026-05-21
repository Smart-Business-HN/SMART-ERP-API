using MediatR;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Specifications.PurchaseOrderSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Queries
{
    public class GetProviderSummaryQuery : IRequest<Response<ProviderSummaryDto>>
    {
        public int Id { get; set; }
    }

    public class GetProviderSummaryQueryHandler : IRequestHandler<GetProviderSummaryQuery, Response<ProviderSummaryDto>>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepository;
        private readonly IRepositoryAsync<PurchaseOrder> _purchaseOrderRepository;

        public GetProviderSummaryQueryHandler(
            IMediator mediator,
            IRepositoryAsync<PurchaseBill> purchaseBillRepository,
            IRepositoryAsync<PurchaseOrder> purchaseOrderRepository)
        {
            _mediator = mediator;
            _purchaseBillRepository = purchaseBillRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<Response<ProviderSummaryDto>> Handle(GetProviderSummaryQuery request, CancellationToken cancellationToken)
        {
            var providerResponse = await _mediator.Send(new GetProviderByIdQuery { Id = request.Id }, cancellationToken);
            if (providerResponse?.Data == null)
            {
                throw new KeyNotFoundException($"Proveedor no encontrado con el id {request.Id}");
            }

            var bills = await _purchaseBillRepository.ListAsync(
                new GetPurchaseBillsByProviderIdForSummarySpecification(request.Id), cancellationToken);

            var totalPurchased = bills.Sum(b => b.Total);
            var outstandingBalance = bills.Sum(b => b.Outstanding);
            var totalPayments = bills
                .SelectMany(b => b.PurchaseBillPayments ?? new List<PurchaseBillPayment>())
                .Sum(p => p.Amount);
            var billCount = bills.Count;
            var averagePurchase = billCount > 0
                ? Math.Round(totalPurchased / billCount, 2)
                : 0m;

            var orders = await _purchaseOrderRepository.ListAsync(
                new GetPurchaseOrdersByProviderIdForSummarySpecification(request.Id), cancellationToken);

            var topProducts = orders
                .SelectMany(o => o.ProductsToPurchase ?? new List<ProductToPurchase>())
                .GroupBy(ptp => new
                {
                    ptp.ProductId,
                    Name = ptp.Product != null ? ptp.Product.Name : ptp.ProductName
                })
                .Select(g => new TopPurchasedProductFromProviderDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name ?? "Sin nombre",
                    QuantityPurchased = g.Sum(x => x.Quantity),
                    Spent = g.Sum(x => x.TotalLine)
                })
                .OrderByDescending(p => p.QuantityPurchased)
                .Take(5)
                .ToList();

            var startDate = DateTime.UtcNow.AddMonths(-11);
            var startMonth = new DateOnly(startDate.Year, startDate.Month, 1);
            var purchaseActivity = bills
                .Where(b => b.InvoiceDate >= startMonth.ToDateTime(TimeOnly.MinValue))
                .GroupBy(b => new DateOnly(b.InvoiceDate.Year, b.InvoiceDate.Month, 1))
                .Select(g => new PurchaseDateCountDto
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(x => x.Total)
                })
                .OrderBy(p => p.Date)
                .ToList();

            var summary = new ProviderSummaryDto
            {
                Provider = providerResponse.Data,
                TotalPurchased = totalPurchased,
                OutstandingBalance = outstandingBalance,
                TotalPayments = totalPayments,
                BillCount = billCount,
                AveragePurchase = averagePurchase,
                TopProducts = topProducts,
                PurchaseActivity = purchaseActivity
            };

            return new Response<ProviderSummaryDto>(summary);
        }
    }
}
