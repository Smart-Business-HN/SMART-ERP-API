namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Estado del asiento contable. Draft = borrador editable; Posted = contabilizado e inmutable;
    /// Reversed = anulado por un asiento de reversa; ReversalEntry = el asiento espejo que anula a otro.
    /// </summary>
    public enum JournalEntryStatus
    {
        Draft = 1,
        Posted = 2,
        Reversed = 3,
        ReversalEntry = 4
    }
}
