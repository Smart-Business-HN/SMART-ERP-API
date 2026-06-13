using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using SMART.ERP.Domain.Settings;

namespace SMART.ERP.Application.Services.RepricingEngine
{
    public class EcommerceRevalidationService : IEcommerceRevalidationService
    {
        private readonly RepricingScraperSettings _settings;
        private readonly ILogger<EcommerceRevalidationService> _logger;

        public EcommerceRevalidationService(
            IOptions<RepricingScraperSettings> settings,
            ILogger<EcommerceRevalidationService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task RevalidateStoreAsync(CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.EcommerceBaseUrl))
                return;

            try
            {
                var url = $"{_settings.EcommerceBaseUrl.TrimEnd('/')}/api/revalidate";
                var options = new RestClientOptions(url) { Timeout = TimeSpan.FromSeconds(15) };
                using var client = new RestClient(options);
                var request = new RestRequest();
                if (!string.IsNullOrWhiteSpace(_settings.RevalidateSecret))
                    request.AddHeader("x-revalidate-secret", _settings.RevalidateSecret);

                await client.PostAsync(request, ct);
            }
            catch (Exception ex)
            {
                // Best-effort: si falla, el ISR igual refresca dentro de la ventana de revalidación.
                _logger.LogWarning(ex, "No se pudo disparar la revalidación del e-commerce.");
            }
        }
    }
}
