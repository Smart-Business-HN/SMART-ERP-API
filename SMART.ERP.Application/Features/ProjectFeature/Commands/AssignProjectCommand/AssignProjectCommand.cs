using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Commands.AssignProjectCommand
{
    public class AssignProjectCommand : IRequest<Response<string>>
    {
        public int ProjectId { get; set; }
        public List<int>? InvoiceIds { get; set; }
        public List<int>? PurchaseBillIds { get; set; }
        public List<int>? NonBillableExpenseIds { get; set; }
        public List<int>? QuotationIds { get; set; }
    }

    public class AssignProjectCommandHandler : IRequestHandler<AssignProjectCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Project> _projectRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<PurchaseBill> _purchaseBillRepositoryAsync;
        private readonly IRepositoryAsync<NonBillableExpense> _nonBillableExpenseRepositoryAsync;
        private readonly IRepositoryAsync<Quotation> _quotationRepositoryAsync;

        public AssignProjectCommandHandler(
            IRepositoryAsync<Project> projectRepositoryAsync,
            IRepositoryAsync<Invoice> invoiceRepositoryAsync,
            IRepositoryAsync<PurchaseBill> purchaseBillRepositoryAsync,
            IRepositoryAsync<NonBillableExpense> nonBillableExpenseRepositoryAsync,
            IRepositoryAsync<Quotation> quotationRepositoryAsync)
        {
            _projectRepositoryAsync = projectRepositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _purchaseBillRepositoryAsync = purchaseBillRepositoryAsync;
            _nonBillableExpenseRepositoryAsync = nonBillableExpenseRepositoryAsync;
            _quotationRepositoryAsync = quotationRepositoryAsync;
        }

        public async Task<Response<string>> Handle(AssignProjectCommand request, CancellationToken cancellationToken)
        {
            var projectExist = await _projectRepositoryAsync.GetByIdAsync(request.ProjectId);
            if (projectExist == null)
            {
                throw new ApiException($"No existe un proyecto con el ID: {request.ProjectId}");
            }

            int assignedCount = 0;

            if (request.InvoiceIds != null && request.InvoiceIds.Count > 0)
            {
                foreach (var invoiceId in request.InvoiceIds)
                {
                    var invoice = await _invoiceRepositoryAsync.GetByIdAsync(invoiceId);
                    if (invoice != null)
                    {
                        invoice.ProjectId = request.ProjectId;
                        await _invoiceRepositoryAsync.UpdateAsync(invoice);
                        assignedCount++;
                    }
                }
                await _invoiceRepositoryAsync.SaveChangesAsync();
            }

            if (request.PurchaseBillIds != null && request.PurchaseBillIds.Count > 0)
            {
                foreach (var purchaseBillId in request.PurchaseBillIds)
                {
                    var purchaseBill = await _purchaseBillRepositoryAsync.GetByIdAsync(purchaseBillId);
                    if (purchaseBill != null)
                    {
                        purchaseBill.ProjectId = request.ProjectId;
                        await _purchaseBillRepositoryAsync.UpdateAsync(purchaseBill);
                        assignedCount++;
                    }
                }
                await _purchaseBillRepositoryAsync.SaveChangesAsync();
            }

            if (request.NonBillableExpenseIds != null && request.NonBillableExpenseIds.Count > 0)
            {
                foreach (var expenseId in request.NonBillableExpenseIds)
                {
                    var expense = await _nonBillableExpenseRepositoryAsync.GetByIdAsync(expenseId);
                    if (expense != null)
                    {
                        expense.ProjectId = request.ProjectId;
                        await _nonBillableExpenseRepositoryAsync.UpdateAsync(expense);
                        assignedCount++;
                    }
                }
                await _nonBillableExpenseRepositoryAsync.SaveChangesAsync();
            }

            if (request.QuotationIds != null && request.QuotationIds.Count > 0)
            {
                foreach (var quotationId in request.QuotationIds)
                {
                    var quotation = await _quotationRepositoryAsync.GetByIdAsync(quotationId);
                    if (quotation != null)
                    {
                        quotation.ProjectId = request.ProjectId;
                        await _quotationRepositoryAsync.UpdateAsync(quotation);
                        assignedCount++;
                    }
                }
                await _quotationRepositoryAsync.SaveChangesAsync();
            }

            return new Response<string>($"{assignedCount} registros asociados al proyecto exitosamente.");
        }
    }
}
