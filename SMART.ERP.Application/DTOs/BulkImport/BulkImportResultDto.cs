namespace SMART.ERP.Application.DTOs.BulkImport
{
    /// <summary>
    /// Resultado de una importacion masiva. En modo todo-o-nada, si <see cref="ErrorCount"/> es
    /// mayor a 0 entonces <see cref="SuccessCount"/> es 0 y no se inserto ningun registro.
    /// </summary>
    public class BulkImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<BulkImportRowError> Errors { get; set; } = [];
    }

    /// <summary>
    /// Error asociado a una fila/columna especifica del archivo importado.
    /// </summary>
    public class BulkImportRowError
    {
        public int Row { get; set; }
        public string Field { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
