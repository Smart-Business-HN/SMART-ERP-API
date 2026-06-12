using System.Globalization;
using ClosedXML.Excel;
using SMART.ERP.Application.Services.VirtualStock;

namespace SMART.ERP.Infrastructure.Services.VirtualStock
{
    public class VirtualStockExcelReader : IVirtualStockExcelReader
    {
        public List<VirtualStockRow> ReadRows(Stream fileStream)
        {
            var rows = new List<VirtualStockRow>();

            using var workbook = new XLWorkbook(fileStream);
            if (workbook.Worksheets.Count == 0) return rows;

            var ws = workbook.Worksheet(1);

            // Columnas esperadas: 1=ProductCode, 2=Quantity, 3=CostPrice (opcional).
            // Se salta la primera fila (encabezado), igual que el importador CSV.
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var productCode = row.Cell(1).GetString().Trim();
                var quantity = CellToInvariantString(row.Cell(2));
                var costPrice = CellToInvariantString(row.Cell(3));

                // Omitir filas completamente vacías.
                if (string.IsNullOrWhiteSpace(productCode) && string.IsNullOrWhiteSpace(quantity))
                    continue;

                rows.Add(new VirtualStockRow
                {
                    RowNumber = row.RowNumber(),
                    ProductCode = productCode,
                    Quantity = quantity,
                    CostPrice = string.IsNullOrWhiteSpace(costPrice) ? null : costPrice
                });
            }

            return rows;
        }

        /// <summary>
        /// Devuelve el valor de la celda como string en formato invariante. Para celdas numéricas usa el
        /// valor decimal (evita problemas de separador decimal/miles según la cultura del archivo), de modo
        /// que VirtualStockService pueda parsear con CultureInfo.InvariantCulture igual que en el CSV.
        /// </summary>
        private static string CellToInvariantString(IXLCell cell)
        {
            if (cell.DataType == XLDataType.Number)
                return cell.GetValue<decimal>().ToString(CultureInfo.InvariantCulture);

            return cell.GetString().Trim();
        }
    }
}
