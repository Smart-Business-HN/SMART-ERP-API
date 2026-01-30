using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.DTOs.Quotation;

namespace SMART.ERP.Application.DTOs.Project
{
    public class ProjectFinancialSummaryDto
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;
        public string ProjectCode { get; set; } = null!;
        public decimal ExecutionBudget { get; set; }
        public decimal TotalPurchaseBills { get; set; }
        public decimal TotalNonBillableExpenses { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal Profit { get; set; }
        public decimal BudgetRemaining { get; set; }
        public int PurchaseBillCount { get; set; }
        public int NonBillableExpenseCount { get; set; }
        public int InvoiceCount { get; set; }
        public int QuotationCount { get; set; }
        public List<PurchaseBillDto>? PurchaseBills { get; set; }
        public List<NonBillableExpenseDto>? NonBillableExpenses { get; set; }
        public List<InvoiceDto>? Invoices { get; set; }
        public List<QuotationDto>? Quotations { get; set; }
    }
}
