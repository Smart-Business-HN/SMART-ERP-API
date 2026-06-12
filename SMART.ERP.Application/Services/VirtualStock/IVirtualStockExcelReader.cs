namespace SMART.ERP.Application.Services.VirtualStock
{
    /// <summary>
    /// Lee un archivo Excel (.xlsx/.xls) de stock virtual y devuelve sus filas crudas.
    /// El parseo/validación de cada fila lo hace VirtualStockService (misma lógica que el CSV).
    /// Implementado en Infrastructure (ClosedXML) por la regla onion: Application no referencia ClosedXML.
    /// </summary>
    public interface IVirtualStockExcelReader
    {
        List<VirtualStockRow> ReadRows(Stream fileStream);
    }

    public class VirtualStockRow
    {
        /// <summary>Número de fila real en la hoja (para mensajes de error).</summary>
        public int RowNumber { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string? CostPrice { get; set; }
    }
}
