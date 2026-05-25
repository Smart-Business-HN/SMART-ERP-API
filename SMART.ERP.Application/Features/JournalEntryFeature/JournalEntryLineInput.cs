namespace SMART.ERP.Application.Features.JournalEntryFeature
{
    /// <summary>Línea de entrada al crear/editar un asiento contable.</summary>
    public class JournalEntryLineInput
    {
        public int LedgerAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public int? ProjectId { get; set; }
        /// <summary>Tercero cliente (excluyente con ProviderId).</summary>
        public Guid? CustomerId { get; set; }
        /// <summary>Tercero proveedor (excluyente con CustomerId).</summary>
        public int? ProviderId { get; set; }
    }
}
