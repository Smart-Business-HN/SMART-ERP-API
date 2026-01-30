using MediatR;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Queries
{
    public class GetUnassignedRecordsQuery : IRequest<PagedResponse<UnassignedRecordsResponseDto>>
    {
        public string RecordType { get; set; } = null!;
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetUnassignedRecordsQueryHandler : IRequestHandler<GetUnassignedRecordsQuery, PagedResponse<UnassignedRecordsResponseDto>>
        {
            private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
            private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
            private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepositoryAsync;
            private readonly IRepositoryAsync<Quotation> _quotationRepositoryAsync;

            public GetUnassignedRecordsQueryHandler(
                IRepositoryAsync<Invoice> invoiceRepositoryAsync,
                IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync,
                IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepositoryAsync,
                IRepositoryAsync<Quotation> quotationRepositoryAsync)
            {
                _invoiceRepositoryAsync = invoiceRepositoryAsync;
                _purchaseBillRepositoryAsync = purchaseBillRepositoryAsync;
                _nonBillableExpenseRepositoryAsync = nonBillableExpenseRepositoryAsync;
                _quotationRepositoryAsync = quotationRepositoryAsync;
            }

            public async Task<PagedResponse<UnassignedRecordsResponseDto>> Handle(GetUnassignedRecordsQuery request, CancellationToken cancellationToken)
            {
                var response = new UnassignedRecordsResponseDto();

                switch (request.RecordType?.ToLower())
                {
                    case "invoice":
                        var invoices = await _invoiceRepositoryAsync.ListAsync(
                            new Specifications.ProjectSpecification.FilterUnassignedInvoicesSpecification(request.Parameter, request.PageNumber, request.PageSize));
                        response.Records = invoices.Select(x => new UnassignedRecordDto
                        {
                            Id = x.Id,
                            Code = x.InvoiceNumber ?? "",
                            Description = x.Customer?.FullName,
                            Total = x.Total
                        }).ToList();
                        response.TotalCount = await _invoiceRepositoryAsync.CountAsync(
                            new Specifications.ProjectSpecification.CountUnassignedInvoicesSpecification(request.Parameter));
                        break;

                    case "purchasebill":
                        var purchaseBills = await _purchaseBillRepositoryAsync.ListAsync(
                            new Specifications.ProjectSpecification.FilterUnassignedPurchaseBillsSpecification(request.Parameter, request.PageNumber, request.PageSize));
                        response.Records = purchaseBills.Select(x => new UnassignedRecordDto
                        {
                            Id = x.Id,
                            Code = x.PurchaseBillCode ?? "",
                            Description = x.Provider?.Name,
                            Total = x.Total
                        }).ToList();
                        response.TotalCount = await _purchaseBillRepositoryAsync.CountAsync(
                            new Specifications.ProjectSpecification.CountUnassignedPurchaseBillsSpecification(request.Parameter));
                        break;

                    case "nonbillableexpense":
                        var expenses = await _nonBillableExpenseRepositoryAsync.ListAsync(
                            new Specifications.ProjectSpecification.FilterUnassignedNonBillableExpensesSpecification(request.Parameter, request.PageNumber, request.PageSize));
                        response.Records = expenses.Select(x => new UnassignedRecordDto
                        {
                            Id = x.Id,
                            Code = x.ExpenseCode ?? "",
                            Description = x.Description,
                            Total = x.Amount
                        }).ToList();
                        response.TotalCount = await _nonBillableExpenseRepositoryAsync.CountAsync(
                            new Specifications.ProjectSpecification.CountUnassignedNonBillableExpensesSpecification(request.Parameter));
                        break;

                    case "quotation":
                        var quotations = await _quotationRepositoryAsync.ListAsync(
                            new Specifications.ProjectSpecification.FilterUnassignedQuotationsSpecification(request.Parameter, request.PageNumber, request.PageSize));
                        response.Records = quotations.Select(x => new UnassignedRecordDto
                        {
                            Id = x.Id,
                            Code = x.QuotationCode ?? "",
                            Description = x.Customer?.FullName,
                            Total = x.Total
                        }).ToList();
                        response.TotalCount = await _quotationRepositoryAsync.CountAsync(
                            new Specifications.ProjectSpecification.CountUnassignedQuotationsSpecification(request.Parameter));
                        break;
                }

                return new PagedResponse<UnassignedRecordsResponseDto>(response, request.PageNumber, request.PageSize, response.TotalCount);
            }
        }
    }
}
