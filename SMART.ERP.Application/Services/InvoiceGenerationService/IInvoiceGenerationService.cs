using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.InvoiceGenerationService
{
    public interface IInvoiceGenerationService
    {
        string CreateInvoiceNumber(Cai cai);
        decimal TaxCalculator(decimal quantity, decimal unitPrice, Tax tax);
        decimal CalculateGravableValue(List<ProductSoldDto> products, Tax tax);
        decimal CalculateTaxesValue(List<ProductSoldDto> products, Tax tax);
    }
}
