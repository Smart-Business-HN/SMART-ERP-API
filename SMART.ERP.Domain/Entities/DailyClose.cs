using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
        [Precision(18, 2)]
        public decimal Exonerated { get; set; }
        [Precision(18, 2)]
        public decimal Exempt { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt15Percent { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes15Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Total { get; set; }
        [Precision(18, 2)]
        public decimal SpotSales { get; set; }
        [Precision(18, 2)]
        public decimal CreditSales { get; set; }
        [Precision(18, 2)]
        public decimal CashSalesPayments { get; set; }
        [Precision(18, 2)]
        public decimal CreditSalesPayments { get; set; }
        [Precision(18, 2)]
        public decimal TotalIncomes { get; set; }
        public int NumberOfInvoices { get; set; }
        [MaxLength(19)]
        public string StartCorrelative { get; set; } = null!;
        [MaxLength(19)]
        public string EndCorrelative { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public List<ResumePayment>? ResumePayments { get; set; }
    }
}
