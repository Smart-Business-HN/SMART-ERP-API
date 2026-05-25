using ClosedXML.Excel;
using SMART.ERP.Application.DTOs.Kardex;
using SMART.ERP.Application.Services.KardexReportService;

namespace SMART.ERP.Infrastructure.Services.KardexReportService
{
    public class KardexExcelService : IKardexExcelService
    {
        public Task<byte[]> GenerateExcelAsync(KardexReportDto report)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Kardex");

            ws.Cell(1, 1).Value = "Kardex de Inventario";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 14;

            ws.Cell(2, 1).Value = "Producto:";
            ws.Cell(2, 2).Value = $"{report.ProductCode} - {report.ProductName}";
            ws.Cell(3, 1).Value = "Bodega:";
            ws.Cell(3, 2).Value = string.IsNullOrEmpty(report.WarehouseName) ? "Todas" : report.WarehouseName;
            ws.Cell(4, 1).Value = "Saldo inicial:";
            ws.Cell(4, 2).Value = report.OpeningQuantity;

            var headerRow = 6;
            string[] headers = ["Fecha", "Tipo", "Documento", "Tercero", "Entra", "Sale", "Costo Unit.", "Saldo", "Costo Prom.", "Valor"];
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");
            }

            var row = headerRow + 1;
            foreach (var m in report.Movements)
            {
                ws.Cell(row, 1).Value = m.MovementDate;
                ws.Cell(row, 1).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(row, 2).Value = m.MovementTypeName;
                ws.Cell(row, 3).Value = m.DocumentCode ?? "-";
                ws.Cell(row, 4).Value = m.ThirdPartyName ?? "-";
                ws.Cell(row, 5).Value = m.QuantityIn;
                ws.Cell(row, 6).Value = m.QuantityOut;
                ws.Cell(row, 7).Value = m.UnitCost ?? 0m;
                ws.Cell(row, 8).Value = m.RunningQuantity;
                ws.Cell(row, 9).Value = m.RunningAverageCost;
                ws.Cell(row, 10).Value = m.RunningTotalValue;
                row++;
            }

            ws.Cell(row + 1, 1).Value = "Saldo final:";
            ws.Cell(row + 1, 1).Style.Font.Bold = true;
            ws.Cell(row + 1, 8).Value = report.ClosingQuantity;
            ws.Cell(row + 1, 9).Value = report.ClosingAverageCost;
            ws.Cell(row + 1, 10).Value = report.ClosingTotalValue;

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return Task.FromResult(stream.ToArray());
        }
    }
}
