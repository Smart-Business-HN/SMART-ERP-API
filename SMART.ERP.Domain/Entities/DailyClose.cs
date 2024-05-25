
namespace SMART.ERP.Domain.Entities
{
    public class DailyClose
    {
        public int Id { get; init; }
        public DateOnly Date { get; set; }
        public int BranchOfficeId { get; set; }
        public BranchOffices? BranchOffice { get; set; }
        public int CaiId { get; set; }
        public Cai? Cai { get; set; }
        public decimal Exonerated { get; set; }
        public decimal Exempt { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
        public decimal Total { get; set; }
        public decimal SpotSales { get; set; }
        public decimal CreditSales { get; set; }
        public decimal CashSalesPayments { get; set; }
        public decimal CreditSalesPayments { get; set; }
        public decimal TotalIncomes { get; set; }
        public int NumberOfInvoices { get; set; }
        public string StartCorrelative { get; set; } = null!;
        public string EndCorrelative { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public List<ResumePayment>? ResumePayments { get; set; }
    }
}
