using SMART.ERP.Application.DTOs.Kardex;

namespace SMART.ERP.Application.Services.KardexReportService
{
    public interface IKardexExcelService
    {
        Task<byte[]> GenerateExcelAsync(KardexReportDto report);
    }
}
