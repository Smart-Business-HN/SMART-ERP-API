using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Services.InventoryMovementService
{
    public interface IInventoryMovementService
    {
        /// <summary>
        /// Registra un movimiento en el Kardex calculando saldo acumulado y costo promedio
        /// ponderado. Opcionalmente sincroniza stock (InventoryDistribution + Product) y el
        /// costo del producto. Debe ejecutarse dentro de una transacción.
        /// </summary>
        Task<InventoryMovement> RecordMovementAsync(RecordMovementInput input, CancellationToken cancellationToken = default);

        /// <summary>
        /// Crea movimientos de reverso para todos los movimientos (no cancelaciones) de un
        /// documento, invirtiendo entradas/salidas y revirtiendo el stock.
        /// </summary>
        Task RecordCancellationForDocumentAsync(string documentType, int documentId, DateTime movementDate, KardexMovementType cancellationType, Guid? userId, string? userName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recalcula Product.CurrentStock como la suma de sus InventoryDistribution.
        /// Garantiza que el total del producto refleje el stock por bodega aunque no se
        /// haya generado un movimiento (p. ej. ajustes con delta cero) o existan datos
        /// heredados desincronizados.
        /// </summary>
        Task SyncProductStockAsync(int productId, CancellationToken cancellationToken = default);

        KardexMovementType MapExitReasonToMovementType(InventoryExitReason reason);
    }
}
