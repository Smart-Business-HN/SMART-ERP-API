using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Helpers
{
    public static class KardexMovementTypeNames
    {
        public static string GetName(KardexMovementType type) => type switch
        {
            KardexMovementType.Purchase => "Compra",
            KardexMovementType.Sale => "Venta",
            KardexMovementType.Adjustment => "Ajuste",
            KardexMovementType.OpeningStock => "Inventario inicial",
            KardexMovementType.TransferOut => "Transferencia (salida)",
            KardexMovementType.TransferIn => "Transferencia (entrada)",
            KardexMovementType.CustomerReturn => "Devolución de cliente",
            KardexMovementType.SupplierReturn => "Devolución a proveedor",
            KardexMovementType.ManualSale => "Venta manual",
            KardexMovementType.InvoiceCancellation => "Anulación de factura",
            KardexMovementType.ManualInvoiceCancellation => "Anulación de venta manual",
            KardexMovementType.Shrinkage => "Merma",
            KardexMovementType.Sample => "Muestra",
            KardexMovementType.Gift => "Obsequio",
            KardexMovementType.Damage => "Daño",
            KardexMovementType.Theft => "Robo",
            KardexMovementType.InternalUse => "Uso interno",
            KardexMovementType.Expiration => "Vencimiento",
            KardexMovementType.OtherExit => "Otra salida",
            KardexMovementType.InventoryExitCancellation => "Anulación de salida",
            KardexMovementType.EntryCancellation => "Anulación de entrada",
            KardexMovementType.TransferCancellation => "Anulación de transferencia",
            _ => type.ToString()
        };
    }
}
