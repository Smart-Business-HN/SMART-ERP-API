using System.Globalization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.BulkImport;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Services.ExcelImportService;

namespace SMART.ERP.Infrastructure.Services.ExcelImportService
{
    /// <summary>
    /// Implementacion ClosedXML del motor de plantillas. Vive en Infrastructure porque
    /// SMART.ERP.Application no referencia ClosedXML (mismo patron que VirtualStockExcelReader).
    /// </summary>
    public class ExcelImportService : IExcelImportService
    {
        public byte[] GenerateTemplate(string sheetName, ExcelColumnDefinition[] columns)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            // Fila de encabezados (fila 1)
            for (int i = 0; i < columns.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = columns[i].HeaderTitle;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = columns[i].IsRequired
                    ? XLColor.FromHtml("#1E40AF")   // Azul (Blue-800) para requeridas
                    : XLColor.FromHtml("#6B7280");   // Gris (Gray-500) para opcionales
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // El valor de ejemplo se adjunta como comentario de celda (NO como fila de datos),
                // para que una plantilla recien descargada y llenada desde la fila 2 no importe el placeholder.
                if (!string.IsNullOrEmpty(columns[i].ExampleValue))
                {
                    cell.GetComment().AddText($"Ejemplo: {columns[i].ExampleValue}");
                }
            }

            // Auto-ajustar columnas
            worksheet.Columns().AdjustToContents();

            // Ancho minimo de columna
            foreach (var column in worksheet.ColumnsUsed())
            {
                if (column.Width < 18)
                    column.Width = 18;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public List<ExcelImportRow> ParseRows(IFormFile file, string expectedSheetName, ExcelColumnDefinition[] columns)
        {
            using var stream = file.OpenReadStream();
            XLWorkbook workbook;

            try
            {
                workbook = new XLWorkbook(stream);
            }
            catch (Exception)
            {
                throw new ApiException("El archivo no es un archivo Excel valido (.xlsx).");
            }

            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw new ApiException("El archivo Excel no contiene hojas de trabajo.");

            // Validar que el nombre de la hoja corresponda a la plantilla esperada
            if (!worksheet.Name.Equals(expectedSheetName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ApiException(
                    $"La plantilla no corresponde a esta seccion. " +
                    $"Se esperaba la plantilla de \"{expectedSheetName}\" pero se recibio \"{worksheet.Name}\". " +
                    $"Por favor descargue la plantilla correcta.");
            }

            // Construir el mapa encabezado -> propiedad desde la fila 1
            var headerMap = new Dictionary<int, string>(); // columnIndex -> propertyName
            var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

            for (int col = 1; col <= lastColumn; col++)
            {
                var headerValue = worksheet.Cell(1, col).GetString().Trim();
                if (string.IsNullOrEmpty(headerValue)) continue;

                var matchingColumn = columns.FirstOrDefault(c =>
                    c.HeaderTitle.Equals(headerValue, StringComparison.OrdinalIgnoreCase)
                    || c.HeaderAliases.Any(a => a.Equals(headerValue, StringComparison.OrdinalIgnoreCase)));

                if (matchingColumn != null)
                {
                    headerMap[col] = matchingColumn.PropertyName;
                }
            }

            // Validar que esten todas las columnas requeridas
            var foundProperties = headerMap.Values.ToHashSet();
            var missingRequired = columns
                .Where(c => c.IsRequired && !foundProperties.Contains(c.PropertyName))
                .Select(c => c.HeaderTitle)
                .ToList();

            if (missingRequired.Count > 0)
            {
                throw new ApiException(
                    $"Faltan las siguientes columnas requeridas: {string.Join(", ", missingRequired)}. " +
                    "Por favor descargue la plantilla y use los encabezados correctos.");
            }

            // Parsear filas de datos (desde la fila 2)
            var rows = new List<ExcelImportRow>();
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            for (int rowNum = 2; rowNum <= lastRow; rowNum++)
            {
                var row = worksheet.Row(rowNum);

                // Saltar filas completamente vacias
                var isEmptyRow = true;
                foreach (var colIndex in headerMap.Keys)
                {
                    if (!row.Cell(colIndex).IsEmpty())
                    {
                        isEmptyRow = false;
                        break;
                    }
                }
                if (isEmptyRow) continue;

                var importRow = new ExcelImportRow { RowNumber = rowNum };
                foreach (var (colIndex, propertyName) in headerMap)
                {
                    var cellValue = CellToInvariantString(row.Cell(colIndex));
                    importRow.Values[propertyName] = string.IsNullOrEmpty(cellValue) ? null : cellValue;
                }

                rows.Add(importRow);
            }

            workbook.Dispose();
            return rows;
        }

        /// <summary>
        /// Devuelve el valor de la celda como string en formato invariante. Para celdas numericas usa
        /// el valor decimal (evita problemas de separador decimal segun la cultura del archivo), de modo
        /// que el handler pueda parsear con CultureInfo.InvariantCulture. Mismo patron que VirtualStockExcelReader.
        /// </summary>
        private static string CellToInvariantString(IXLCell cell)
        {
            if (cell.DataType == XLDataType.Number)
                return cell.GetValue<decimal>().ToString(CultureInfo.InvariantCulture);

            return cell.GetString().Trim();
        }
    }
}
