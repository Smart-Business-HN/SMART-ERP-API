using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetTopCustomersOutstandingQuery : IRequest<Response<TopCustomersOutstandingDto>>
    {
        public int Top { get; set; } = 5;
    }

    public class GetTopCustomersOutstandingQueryHandler : IRequestHandler<GetTopCustomersOutstandingQuery, Response<TopCustomersOutstandingDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;

        public GetTopCustomersOutstandingQueryHandler(IRepositoryAsync<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Response<TopCustomersOutstandingDto>> Handle(GetTopCustomersOutstandingQuery request, CancellationToken cancellationToken)
        {
            var pendingInvoices = await _invoiceRepository.ListAsync(
                new FilterInvoiceByPendingWithCustomerSpecification(), cancellationToken);

            var customerGroups = pendingInvoices
                .GroupBy(i => new { i.CustomerId, CustomerName = i.Customer?.FullName ?? "Desconocido" })
                .Select(g => new CustomerOutstandingDto
                {
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    TotalOutstanding = g.Sum(i => i.Outstanding),
                    InvoiceCount = g.Count(),
                    OldestInvoiceDate = g.Min(i => i.CreationDate)
                })
                .OrderByDescending(c => c.TotalOutstanding)
                .Take(request.Top)
                .ToList();

            return new Response<TopCustomersOutstandingDto>(new TopCustomersOutstandingDto { Customers = customerGroups });
        }
    }
}
