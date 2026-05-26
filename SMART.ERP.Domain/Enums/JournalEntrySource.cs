namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Origen del asiento. Manual = capturado por el usuario (Fase 1). El resto se reserva para
    /// el posteo automático desde documentos operativos (Fase 2) vía IAccountingPostingService.
    /// </summary>
    public enum JournalEntrySource
    {
        Manual = 1,
        Invoice = 2,
        PurchaseBill = 3,
        BillPayment = 4,
        PurchaseBillPayment = 5,
        NonBillableExpense = 6,
        Kardex = 7,
        YearEndClosing = 8,
        OpeningBalance = 9,
        CreditCardPayment = 10
    }
}
