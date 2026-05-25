using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMART.ERP.Application.DTOs.Kardex;
using SMART.ERP.Application.Services.KardexReportService;

namespace SMART.ERP.Infrastructure.Services.KardexReportService
{
    public class KardexPdfService : IKardexPdfService
    {
        public Task<byte[]> GeneratePdfAsync(KardexReportDto report)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter.Landscape());
                    page.MarginHorizontal(30);
                    page.MarginVertical(25);
                    page.DefaultTextStyle(x => x.FontSize(8).FontColor("#555555"));

                    page.Header().Element(c => ComposeHeader(c, report));
                    page.Content().Element(c => ComposeContent(c, report));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            return Task.FromResult(document.GeneratePdf());
        }

        private static void ComposeHeader(IContainer container, KardexReportDto report)
        {
            container.Column(column =>
            {
                column.Item().Text("Kardex de Inventario").Bold().FontSize(14).FontColor("#1E293B");
                column.Item().Text($"Producto: {report.ProductCode} - {report.ProductName}");
                column.Item().Text($"Bodega: {(string.IsNullOrEmpty(report.WarehouseName) ? "Todas" : report.WarehouseName)}");
                if (report.StartDate.HasValue || report.EndDate.HasValue)
                {
                    var from = report.StartDate?.ToString("dd/MM/yyyy") ?? "Inicio";
                    var to = report.EndDate?.ToString("dd/MM/yyyy") ?? "Hoy";
                    column.Item().Text($"Período: {from} - {to}");
                }
                column.Item().PaddingVertical(4).LineHorizontal(1).LineColor("#000000");
            });
        }

        private static void ComposeContent(IContainer container, KardexReportDto report)
        {
            container.Column(column =>
            {
                column.Item().Text($"Saldo inicial: {report.OpeningQuantity:N2} unidades | Costo prom.: {report.OpeningAverageCost:N4} | Valor: {report.OpeningTotalValue:N2}").FontSize(8).Bold();
                column.Item().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);   // Fecha
                        columns.RelativeColumn(2.5f); // Tipo
                        columns.RelativeColumn(2);   // Documento
                        columns.RelativeColumn(1.2f); // Entra
                        columns.RelativeColumn(1.2f); // Sale
                        columns.RelativeColumn(1.5f); // Costo unit.
                        columns.RelativeColumn(1.5f); // Saldo
                        columns.RelativeColumn(1.5f); // Costo prom.
                        columns.RelativeColumn(1.7f); // Valor
                    });

                    table.Header(header =>
                    {
                        void H(string t) => header.Cell().Background("#E2E8F0").Padding(3).Text(t).Bold().FontSize(8);
                        H("Fecha"); H("Tipo"); H("Documento"); H("Entra"); H("Sale"); H("Costo U."); H("Saldo"); H("Costo Prom."); H("Valor");
                    });

                    foreach (var m in report.Movements)
                    {
                        void C(string t) => table.Cell().BorderBottom(0.5f).BorderColor("#E2E8F0").Padding(3).Text(t).FontSize(7);
                        C(m.MovementDate.ToString("dd/MM/yyyy"));
                        C(m.MovementTypeName);
                        C(m.DocumentCode ?? "-");
                        C(m.QuantityIn > 0 ? m.QuantityIn.ToString("N2") : "-");
                        C(m.QuantityOut > 0 ? m.QuantityOut.ToString("N2") : "-");
                        C(m.UnitCost?.ToString("N4") ?? "-");
                        C(m.RunningQuantity.ToString("N2"));
                        C(m.RunningAverageCost.ToString("N4"));
                        C(m.RunningTotalValue.ToString("N2"));
                    }
                });

                column.Item().PaddingTop(6).Text($"Saldo final: {report.ClosingQuantity:N2} unidades | Costo prom.: {report.ClosingAverageCost:N4} | Valor: {report.ClosingTotalValue:N2}").Bold();
                column.Item().Text($"Total entradas: {report.TotalIn:N2} | Total salidas: {report.TotalOut:N2}").FontSize(8);
            });
        }
    }
}
