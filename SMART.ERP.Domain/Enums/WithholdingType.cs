namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Tipos de retención que el comprador aplica al proveedor al pagar una compra o gasto en Honduras.
    /// El monto retenido se acredita a un pasivo fiscal (Retenciones por pagar) y disminuye lo neto a pagar al proveedor.
    /// </summary>
    public enum WithholdingType
    {
        None = 0,
        /// <summary>Retención ISR 12.5% sobre honorarios profesionales / subcontratistas (2104001).</summary>
        ISR12_5 = 1,
        /// <summary>Retención ISR 1% sobre bienes y servicios (2104002).</summary>
        ISR1 = 2,
        /// <summary>Retención ISV 15% Art. 13 sobre profesionales (2104003).</summary>
        ISV15 = 3
    }
}
