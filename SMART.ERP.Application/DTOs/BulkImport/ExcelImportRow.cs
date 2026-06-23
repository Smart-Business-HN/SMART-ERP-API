namespace SMART.ERP.Application.DTOs.BulkImport
{
    /// <summary>
    /// Una fila de datos leida de la plantilla. <see cref="Values"/> mapea PropertyName -> valor de
    /// celda (string invariante; null si la celda esta vacia). <see cref="RowNumber"/> es el numero
    /// de fila real del Excel (base 1), util para reportar errores al usuario.
    /// </summary>
    public class ExcelImportRow
    {
        public int RowNumber { get; set; }
        public Dictionary<string, string?> Values { get; set; } = new();
    }
}
