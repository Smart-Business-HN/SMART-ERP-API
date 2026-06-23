namespace SMART.ERP.Application.DTOs.BulkImport
{
    /// <summary>
    /// Define una columna de una plantilla de importacion Excel: como se titula el encabezado,
    /// a que propiedad mapea, si es obligatoria y un valor de ejemplo de referencia.
    /// </summary>
    public class ExcelColumnDefinition
    {
        public string HeaderTitle { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public bool IsRequired { get; set; }
        public string? ExampleValue { get; set; }

        /// <summary>
        /// Encabezados alternativos que tambien se aceptan al leer (para compatibilidad con
        /// plantillas viejas). El <see cref="HeaderTitle"/> es el que se usa al generar la plantilla nueva.
        /// </summary>
        public string[] HeaderAliases { get; set; } = Array.Empty<string>();
    }
}
