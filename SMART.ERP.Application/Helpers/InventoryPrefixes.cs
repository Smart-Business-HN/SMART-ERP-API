namespace SMART.ERP.Application.Helpers
{
    /// <summary>
    /// Formatos de prefijo (folio) por tipo de documento de inventario. El backend resuelve
    /// el Prefix por estos formatos para que el usuario no tenga que elegirlo.
    /// </summary>
    public static class InventoryPrefixes
    {
        public const string Entry = "IE";    // Entrada de Inventario
        public const string Exit = "IO";     // Salida de Inventario
        public const string Transfer = "WT"; // Transferencia de Inventario (Warehouse Transfer)
    }
}
