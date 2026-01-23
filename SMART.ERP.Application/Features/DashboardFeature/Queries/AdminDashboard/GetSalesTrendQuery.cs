using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Globalization;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetSalesTrendQuery : IRequest<Response<SalesTrendDto>>
    {
        public int Months { get; set; } = 12;
    }

    public class GetSalesTrendQueryHandler : IRequestHandler<GetSalesTrendQuery, Response<SalesTrendDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;

        public GetSalesTrendQueryHandler(IRepositoryAsync<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Response<SalesTrendDto>> Handle(GetSalesTrendQuery request, CancellationToken cancellationToken)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-request.Months + 1);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);

            var invoices = await _invoiceRepository.ListAsync(
                new FilterInvoiceByDateRangeSpecification(startDate, endDate), cancellationToken);

            var result = new SalesTrendDto();
            var culture = new CultureInfo("es-HN");

            for (int i = 0; i < request.Months; i++)
            {
                var monthDate = startDate.AddMonths(i);
                var monthInvoices = invoices
                    .Where(inv => inv.CreationDate.Year == monthDate.Year && inv.CreationDate.Month == monthDate.Month)
                    .ToList();

                result.Months.Add(new SalesMonthDto
                {
                    Year = monthDate.Year,
                    Month = monthDate.Month,
                    MonthName = culture.DateTimeFormat.GetAbbreviatedMonthName(monthDate.Month),
                    TotalSales = monthInvoices.Sum(inv => inv.Total),
                    InvoiceCount = monthInvoices.Count
                });
            }

            return new Response<SalesTrendDto>(result);
        }
    }
}
