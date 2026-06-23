using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.BulkImport;

namespace SMART.ERP.Application.Services.ExcelImportService
{
    /// <summary>
    /// Motor reutilizable de plantillas Excel para importaciones masivas. Genera plantillas con
    /// encabezados estilizados y parsea archivos subidos a una lista de filas tipadas por nombre de
    /// columna. La implementacion (ClosedXML) vive en Infrastructure.
    /// </summary>
    public interface IExcelImportService
    {
        /// <summary>
        /// Genera una plantilla .xlsx con una fila de encabezados (azul = requerido, gris = opcional).
        /// NO escribe una fila de ejemplo: los <c>ExampleValue</c> solo guian el contenido de cada columna.
        /// </summary>
        byte[] GenerateTemplate(string sheetName, ExcelColumnDefinition[] columns);

        /// <summary>
        /// Lee el archivo subido validando que la hoja corresponda a la plantilla esperada y que esten
        /// todas las columnas requeridas. Devuelve una fila por cada fila de datos no vacia.
        /// </summary>
        List<ExcelImportRow> ParseRows(IFormFile file, string expectedSheetName, ExcelColumnDefinition[] columns);
    }
}
