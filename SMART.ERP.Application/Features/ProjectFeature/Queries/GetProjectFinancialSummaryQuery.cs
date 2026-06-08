using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProjectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.ProjectFeature.Queries
{
    public class GetProjectFinancialSummaryQuery : IRequest<Response<ProjectFinancialSummaryDto>>
    {
        public int Id { get; set; }
    }

    public class GetProjectFinancialSummaryQueryHandler : IRequestHandler<GetProjectFinancialSummaryQuery, Response<ProjectFinancialSummaryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Project> _repositoryAsync;

        public GetProjectFinancialSummaryQueryHandler(IMapper mapper, IRepositoryAsync<Project> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ProjectFinancialSummaryDto>> Handle(GetProjectFinancialSummaryQuery request, CancellationToken cancellationToken)
        {
            var project = await _repositoryAsync.FirstOrDefaultAsync(new FilterProjectByIdSpecification(request.Id));
            if (project == null)
            {
                throw new KeyNotFoundException($"Proyecto no encontrado con el id {request.Id}");
            }

            // Solo las salidas CONFIRMADAS imputan gasto al proyecto (las Borrador no afectan stock ni contabilidad,
            // y las Canceladas quedan excluidas). El costo se toma del UnitCost fijado al confirmar la salida.
            var confirmedExits = project.InventoryExits?
                .Where(e => e.Status == InventoryExitStatus.Confirmed)
                .ToList() ?? new List<InventoryExit>();

            var totalPurchaseBills = project.PurchaseBills?.Sum(x => x.Total) ?? 0;
            var totalNonBillableExpenses = project.NonBillableExpenses?.Sum(x => x.Amount) ?? 0;
            var totalInventoryExits = confirmedExits
                .Sum(e => e.Items?.Sum(i => i.Quantity * (i.UnitCost ?? 0)) ?? 0);
            var totalInvested = totalPurchaseBills + totalNonBillableExpenses + totalInventoryExits;
            var totalRevenue = project.Invoices?.Sum(x => x.Total) ?? 0;
            var profit = totalRevenue - totalInvested;
            var budgetRemaining = project.ExecutionBudget - totalInvested;

            var summary = new ProjectFinancialSummaryDto
            {
                Id = project.Id,
                ProjectName = project.Name,
                ProjectCode = project.ProjectCode,
                ExecutionBudget = project.ExecutionBudget,
                TotalPurchaseBills = totalPurchaseBills,
                TotalNonBillableExpenses = totalNonBillableExpenses,
                TotalInventoryExits = totalInventoryExits,
                TotalInvested = totalInvested,
                TotalRevenue = totalRevenue,
                Profit = profit,
                BudgetRemaining = budgetRemaining,
                PurchaseBillCount = project.PurchaseBills?.Count ?? 0,
                NonBillableExpenseCount = project.NonBillableExpenses?.Count ?? 0,
                InvoiceCount = project.Invoices?.Count ?? 0,
                QuotationCount = project.Quotations?.Count ?? 0,
                InventoryExitCount = confirmedExits.Count,
                PurchaseBills = _mapper.Map<List<PurchaseBillDto>>(project.PurchaseBills),
                NonBillableExpenses = _mapper.Map<List<NonBillableExpenseDto>>(project.NonBillableExpenses),
                Invoices = _mapper.Map<List<InvoiceDto>>(project.Invoices),
                Quotations = _mapper.Map<List<QuotationDto>>(project.Quotations),
                InventoryExits = _mapper.Map<List<InventoryExitDto>>(confirmedExits)
            };

            return new Response<ProjectFinancialSummaryDto>(summary);
        }
    }
}
