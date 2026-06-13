namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Resultado de una evaluación de re-fijación de precio (auditoría en <see cref="Entities.PriceChangeLog"/>).
    /// </summary>
    public enum PriceChangeStatus
    {
        /// <summary>El nuevo precio se aplicó a la lista por defecto.</summary>
        Applied = 1,
        /// <summary>No se aplicó porque el cambio fue menor al umbral mínimo (ruido).</summary>
        Skipped = 2,
        /// <summary>Se quedó en el piso de margen: igualar al competidor habría roto el margen mínimo.</summary>
        FloorHeld = 3,
        /// <summary>Bloqueado por la guarda anti-espiral (caída mayor al máximo permitido por corrida).</summary>
        RejectedByGuard = 4,
        /// <summary>No se pudo leer ningún competidor (todas las fuentes fallaron o estaban agotadas).</summary>
        ScrapeFailed = 5,
        /// <summary>Calculado pero pendiente de aprobación manual (AutoApply desactivado).</summary>
        AwaitingApproval = 6
    }
}
