namespace SMART.ERP.Application.DTOs.VirtualStock
{
    // Proyección ligera para la pestaña "Historial de Importaciones" del panel admin.
    // Los nombres de propiedad coinciden con la interfaz VirtualStockImport del frontend
    // (importedCount / errorCount) para evitar mapeos extra en el cliente.
    public class VirtualStockImportHistoryItemDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime ImportDate { get; set; }
        public int ImportedCount { get; set; }
        public int ErrorCount { get; set; }
        public string ImportedBy { get; set; } = null!;
    }
}
