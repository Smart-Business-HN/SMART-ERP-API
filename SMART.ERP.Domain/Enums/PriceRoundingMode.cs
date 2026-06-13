namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Redondeo aplicado al precio objetivo antes de escribirlo.
    /// </summary>
    public enum PriceRoundingMode
    {
        /// <summary>Sin redondeo (dos decimales).</summary>
        None = 1,
        /// <summary>Termina en .99 (charm pricing): ej. 7234.10 → 7233.99.</summary>
        Charm99 = 2,
        /// <summary>Redondea al lempira entero más cercano hacia abajo.</summary>
        WholeNumber = 3
    }
}
