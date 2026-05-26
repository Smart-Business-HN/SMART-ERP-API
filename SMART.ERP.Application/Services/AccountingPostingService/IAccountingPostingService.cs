namespace SMART.ERP.Application.Services.AccountingPostingService
{
    /// <summary>
    /// Posteo automático de asientos contables desde documentos operativos (Fase 2).
    /// Todos los métodos son no-op si el posteo automático está desactivado en AccountingSettings.
    /// La idempotencia y la reversa se apoyan en JournalEntry.SourceDocumentType/SourceDocumentId.
    /// Cuando está activo y falta configuración (período cerrado, mapeo faltante o cuenta no
    /// imputable), lanzan ApiException para que la operación falle de forma visible.
    /// </summary>
    public interface IAccountingPostingService
    {
        Task PostInvoiceAsync(int invoiceId, CancellationToken cancellationToken);
        /// <summary>
        /// Posteo del devengo mensual para facturas marcadas como IsDeferredRevenue.
        /// Mueve una fracción de Ingresos Diferidos (pasivo) a Ingreso SaaS (resultado).
        /// Idempotente por mes vía SourceDocumentType="Invoice.Recognition" + SourceDocumentId="{invoiceId}.{periodIndex}".
        /// Devuelve true si generó asiento, false si ya estaba reconocido o no aplica.
        /// </summary>
        Task<bool> PostInvoiceRevenueRecognitionAsync(int invoiceId, CancellationToken cancellationToken);
        Task PostBillPaymentAsync(int billPaymentId, CancellationToken cancellationToken);
        Task PostPurchaseBillAsync(int purchaseBillId, CancellationToken cancellationToken);
        Task PostPurchaseBillPaymentAsync(int purchaseBillPaymentId, CancellationToken cancellationToken);
        Task PostNonBillableExpenseAsync(int nonBillableExpenseId, CancellationToken cancellationToken);
        Task PostNonBillableExpensePaymentAsync(int nonBillableExpensePaymentId, CancellationToken cancellationToken);
        Task PostCreditCardPaymentAsync(int creditCardPaymentId, CancellationToken cancellationToken);
        Task PostInventoryEntryAsync(int inventoryEntryId, CancellationToken cancellationToken);
        Task PostInventoryExitAsync(int inventoryExitId, CancellationToken cancellationToken);

        /// <summary>Reversa el asiento generado para un documento (al anular/eliminar).</summary>
        Task ReverseDocumentPostingAsync(string documentType, int documentId, CancellationToken cancellationToken);
    }
}
