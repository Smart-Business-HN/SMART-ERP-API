using SMART.ERP.Application.DTOs.Kardex;

namespace SMART.ERP.Application.Services.KardexReportService
{
    public interface IKardexPdfService
    {
        Task<byte[]> GeneratePdfAsync(KardexReportDto report);
    }
}
