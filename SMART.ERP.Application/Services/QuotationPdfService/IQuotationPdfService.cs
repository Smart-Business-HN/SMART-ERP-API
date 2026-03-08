using SMART.ERP.Application.DTOs.Quotation;

namespace SMART.ERP.Application.Services.QuotationPdfService
{
    public interface IQuotationPdfService
    {
        Task<byte[]> GeneratePdfAsync(QuotationPreviewDto quotation);
    }
}
