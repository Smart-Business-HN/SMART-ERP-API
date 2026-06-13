namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Base impositiva del precio observado en una fuente de competencia, para normalizar la comparación.
    /// </summary>
    public enum CompetitorTaxBasis
    {
        /// <summary>El precio mostrado ya incluye ISV 15% (caso típico de retail en Honduras).</summary>
        IncludesIsv15 = 1,
        /// <summary>El precio mostrado es sin impuestos.</summary>
        ExcludesTax = 2
    }
}
