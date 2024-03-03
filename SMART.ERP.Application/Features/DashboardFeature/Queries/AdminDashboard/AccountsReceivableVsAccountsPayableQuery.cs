using MediatR;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Application.Specifications.PurchaseBillSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.AdminDashboard
{
    public class AccountsReceivableVsAccountsPayableQuery : IRequest<Response<ComparativeRecivableVsPayableDto>>
    {
    }
    public class AccountsReceivableVsAccountsPayableQueryHandle: IRequestHandler<AccountsReceivableVsAccountsPayableQuery,Response<ComparativeRecivableVsPayableDto>>
    {
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        public AccountsReceivableVsAccountsPayableQueryHandle(IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync, IRepositoryAsync<Invoice> invoiceRepositoryAsync)
        {
            _purchaseBillRepositoryAsync = purchaseBillRepositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
        }
        public async Task<Response<ComparativeRecivableVsPayableDto>> Handle(AccountsReceivableVsAccountsPayableQuery request, CancellationToken cancellationToken)
        {
            var purchaseBillsPendingToPay = await _purchaseBillRepositoryAsync.ListAsync(new FilterPurchaseBillByPendingValuesSpecification());
            var invoicesPendindToPay = await _invoiceRepositoryAsync.ListAsync(new FilterInvoiceByPendingValuesSpecification());
            decimal payable = purchaseBillsPendingToPay.Sum(purchaseBill => purchaseBill.Outstanding);
            decimal receivable = invoicesPendindToPay.Sum(invoice => invoice.Outstanding);
            var values = new ComparativeRecivableVsPayableDto
            {
                Payable = payable,
                Recivable = receivable
            };
            return new Response<ComparativeRecivableVsPayableDto>(values);
        }
    }
}
