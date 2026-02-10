using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class DeclaredSaleInvoice
    {
        public int Id { get; init; }
        public int MonthlySaleDeclarationId { get; set; }
        public virtual MonthlySaleDeclaration? MonthlySaleDeclaration { get; set; }
        public int InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        [MaxLength(50)]
        public string CustomerRTN { get; set; } = null!;
        [MaxLength(250)]
        public string CustomerName { get; set; } = null!;
        [MaxLength(10)]
        public string InvoiceDate { get; set; } = null!;
        [MaxLength(37)]
        public string Cai { get; set; } = null!;
        [MaxLength(50)]
        public string CaiName { get; set; } = null!;
        [MaxLength(3)]
        public string Establishment { get; set; } = null!;
        [MaxLength(3)]
        public string EmissionPoint { get; set; } = null!;
        [MaxLength(2)]
        public string KindOfDocument { get; set; } = null!;
        [MaxLength(8)]
        public string Correlative { get; set; } = null!;
        [MaxLength(2)]
        public string SaleWithExoneration { get; set; } = null!;
        [Precision(18, 2)]
        public decimal Exempt { get; set; }
        [Precision(18, 2)]
        public decimal Exonerated { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt15Percent { get; set; }
        [Precision(18, 2)]
        public decimal TaxedAt18Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes15Percent { get; set; }
        [Precision(18, 2)]
        public decimal Taxes18Percent { get; set; }
    }
}
