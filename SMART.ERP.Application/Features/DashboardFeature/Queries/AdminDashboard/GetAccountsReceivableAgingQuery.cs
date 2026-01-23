using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class GetAccountsReceivableAgingQuery : IRequest<Response<AccountsReceivableAgingDto>>
    {
    }

    public class GetAccountsReceivableAgingQueryHandler : IRequestHandler<GetAccountsReceivableAgingQuery, Response<AccountsReceivableAgingDto>>
    {
        private readonly IRepositoryAsync<Invoice> _invoiceRepository;

        public GetAccountsReceivableAgingQueryHandler(IRepositoryAsync<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Response<AccountsReceivableAgingDto>> Handle(GetAccountsReceivableAgingQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var pendingInvoices = await _invoiceRepository.ListAsync(new FilterInvoiceByPendingWithCustomerSpecification(), cancellationToken);

            var result = new AccountsReceivableAgingDto();

            foreach (var invoice in pendingInvoices)
            {
                var dueDate = invoice.ExpectedPaymentDate?.ToDateTime(TimeOnly.MinValue) ?? invoice.CreationDate;
                var daysOverdue = (today - dueDate).Days;
                var bucket = GetAgingBucket(daysOverdue);

                switch (bucket)
                {
                    case "0-30": result.Current += invoice.Outstanding; break;
                    case "31-60": result.Days31To60 += invoice.Outstanding; break;
                    case "61-90": result.Days61To90 += invoice.Outstanding; break;
                    default: result.Over90Days += invoice.Outstanding; break;
                }

                result.Details.Add(new AgingDetailDto
                {
                    EntityId = invoice.CustomerId.ToString(),
                    EntityName = invoice.Customer?.FullName ?? "Desconocido",
                    Outstanding = invoice.Outstanding,
                    DaysOverdue = Math.Max(0, daysOverdue),
                    AgingBucket = bucket
                });
            }

            result.Total = result.Current + result.Days31To60 + result.Days61To90 + result.Over90Days;
            return new Response<AccountsReceivableAgingDto>(result);
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
