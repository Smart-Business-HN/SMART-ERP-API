using MediatR;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CustomerFeature.Queries
{
    public class GetCustomerSummaryQuery : IRequest<Response<CustomerSummaryDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerSummaryQueryHandler : IRequestHandler<GetCustomerSummaryQuery, Response<CustomerSummaryDto>>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;

        public GetCustomerSummaryQueryHandler(IMediator mediator, IRepositoryAsync<Invoice> invoiceRepository)
        {
            _mediator = mediator;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Response<CustomerSummaryDto>> Handle(GetCustomerSummaryQuery request, CancellationToken cancellationToken)
        {
            var customerResponse = await _mediator.Send(new GetCustomerByIdQuery { Id = request.Id }, cancellationToken);
            if (customerResponse?.Data == null)
            {
                throw new KeyNotFoundException($"Cliente no encontrado con el id {request.Id}");
            }

            var invoices = await _invoiceRepository.ListAsync(
                new GetInvoicesByCustomerIdForSummarySpecification(request.Id), cancellationToken);

            var totalInvoiced = invoices.Sum(i => i.Total);
            var outstandingBalance = invoices.Sum(i => i.Outstanding);
            var totalPayments = invoices
                .SelectMany(i => i.BillPayments ?? new List<BillPayment>())
                .Sum(p => p.Amount);
            var invoiceCount = invoices.Count;
            var averagePurchase = invoiceCount > 0
                ? Math.Round(totalInvoiced / invoiceCount, 2)
                : 0m;

            var topProducts = invoices
                .SelectMany(i => i.ProductsSold ?? new List<ProductSold>())
                .GroupBy(ps => new
                {
                    ps.ProductId,
                    Name = ps.Product != null ? ps.Product.Name : ps.ProductDescription
                })
                .Select(g => new TopPurchasedProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name ?? "Sin nombre",
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.TotalLine)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(5)
                .ToList();

            var startDate = DateTime.UtcNow.AddMonths(-11);
            var startMonth = new DateOnly(startDate.Year, startDate.Month, 1);
            var purchaseActivity = invoices
                .Where(i => i.CreationDate >= startMonth.ToDateTime(TimeOnly.MinValue))
                .GroupBy(i => new DateOnly(i.CreationDate.Year, i.CreationDate.Month, 1))
                .Select(g => new PurchaseDateCountDto
                {
                    Date = g.Key,
                    Count = g.Count(),
                    Total = g.Sum(x => x.Total)
                })
                .OrderBy(p => p.Date)
                .ToList();

            var summary = new CustomerSummaryDto
            {
                Customer = customerResponse.Data,
                TotalInvoiced = totalInvoiced,
                OutstandingBalance = outstandingBalance,
                TotalPayments = totalPayments,
                InvoiceCount = invoiceCount,
                AveragePurchase = averagePurchase,
                TopProducts = topProducts,
                PurchaseActivity = purchaseActivity
            };

            return new Response<CustomerSummaryDto>(summary);
        }
    }
}
