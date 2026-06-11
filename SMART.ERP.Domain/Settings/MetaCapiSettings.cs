namespace SMART.ERP.Domain.Settings
{
    // Configuración del webhook de Conversions API (Purchase). El backend reenvía
    // los datos de la compra confirmada al endpoint del ecommerce Next.js, que es
    // quien posee el token de CAPI y realiza la llamada al Graph API de Meta.
    public class MetaCapiSettings
    {
        // URL del route handler de Next.js, p. ej.
        // https://www.smartbusiness.site/api/events/purchase
        public string PurchaseWebhookUrl { get; set; } = null!;

        // Debe coincidir con BACKEND_SECRET_KEY del ecommerce.
        public string BackendSecretKey { get; set; } = null!;
    }
}
