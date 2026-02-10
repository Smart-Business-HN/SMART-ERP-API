using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.MonthlySaleDeclaration;

namespace SMART.ERP.Application.DTOs.DeclaredSaleInvoice
{
    public class DeclaredSaleInvoiceDto
    {
        public int Id { get; init; }
        public int MonthlySaleDeclarationId { get; set; }
        public virtual MonthlySaleDeclarationDto? MonthlySaleDeclaration { get; set; }
        public int InvoiceId { get; set; }
        public virtual InvoiceDto? Invoice { get; set; }
        public string CustomerRTN { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string InvoiceDate { get; set; } = null!;
        public string Cai { get; set; } = null!;
        public string CaiName { get; set; } = null!;
        public string Establishment { get; set; } = null!;
        public string EmissionPoint { get; set; } = null!;
        public string KindOfDocument { get; set; } = null!;
        public string Correlative { get; set; } = null!;
        public string SaleWithExoneration { get; set; } = null!;
        public decimal Exempt { get; set; }
        public decimal Exonerated { get; set; }
        public decimal TaxedAt15Percent { get; set; }
        public decimal TaxedAt18Percent { get; set; }
        public decimal Taxes15Percent { get; set; }
        public decimal Taxes18Percent { get; set; }
    }
}
