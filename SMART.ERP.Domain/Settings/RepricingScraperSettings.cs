namespace SMART.ERP.Domain.Settings
{
    /// <summary>
    /// Configuración de infraestructura del scraper de competencia (sección "RepricingScraper" en appsettings).
    /// Los toggles de negocio viven en la entidad <see cref="Entities.RepricingSettings"/>.
    /// </summary>
    public class RepricingScraperSettings
    {
        /// <summary>User-Agent de navegador real para las peticiones HTTP.</summary>
        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0 Safari/537.36";

        /// <summary>Timeout por petición en segundos.</summary>
        public int TimeoutSeconds { get; set; } = 20;

        /// <summary>Pausa de cortesía entre peticiones al mismo host, en milisegundos.</summary>
        public int PolitenessDelayMs { get; set; } = 1500;

        /// <summary>Base URL del e-commerce para disparar la revalidación ISR tras aplicar un precio.</summary>
        public string? EcommerceBaseUrl { get; set; }

        /// <summary>Secreto compartido para el endpoint /api/revalidate del e-commerce.</summary>
        public string? RevalidateSecret { get; set; }

        /// <summary>Correo que recibe el reporte de estatus del job de re-fijación. Vacío = no se envía.</summary>
        public string? ReportEmailTo { get; set; } = "josec@smartbusiness.site";
    }
}
