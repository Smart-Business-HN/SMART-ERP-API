namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Cómo se calcula cuánto quedar por debajo del competidor más bajo.
    /// </summary>
    public enum UndercutMode
    {
        /// <summary>Porcentaje: target = min(competidores) × (1 − UndercutValue). Ej. 0.01 = 1%.</summary>
        Percent = 1,
        /// <summary>Monto fijo: target = min(competidores) − UndercutValue. Ej. 50 = L.50.</summary>
        Amount = 2
    }
}
