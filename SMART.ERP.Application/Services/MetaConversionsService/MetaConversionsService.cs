using Microsoft.Extensions.Options;
using RestSharp;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Settings;
using System.Text.Json;

namespace SMART.ERP.Application.Services.MetaConversionsService
{
    public class MetaConversionsService : IMetaConversionsService
    {
        private readonly MetaCapiSettings _settings;

        public MetaConversionsService(IOptions<MetaCapiSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendPurchaseAsync(Cart cart, CancellationToken cancellationToken = default)
        {
            // Sin configuración (p. ej. dev): no-op para no romper la verificación.
            if (string.IsNullOrWhiteSpace(_settings.PurchaseWebhookUrl) ||
                string.IsNullOrWhiteSpace(_settings.BackendSecretKey))
            {
                return false;
            }

            var user = cart.EcommerceUser;
            var items = cart.CartItems ?? new List<CartItem>();

            var matchedItems = items.Where(i => i.Product != null).ToList();

            var contents = matchedItems
                .Select(i => new
                {
                    id = i.Product!.Code,
                    quantity = i.Quantity,
                    item_price = i.UnitPrice
                })
                .ToList();

            // value = subtotal de mercancía, consistente con item_price (sin impuesto).
            // Usa el mismo conjunto filtrado que contents para que value == sum(contents).
            var value = matchedItems.Sum(i => i.UnitPrice * i.Quantity);

            var payload = new
            {
                eventId = cart.Id.ToString(),
                orderId = cart.Id.ToString(),
                value,
                currency = "HNL",
                email = user?.Email,
                phone = user?.PhoneNumber,
                firstName = user?.FirstName,
                lastName = user?.LastName,
                contents
            };

            var jsonString = JsonSerializer.Serialize(payload);

            try
            {
                var options = new RestClientOptions(_settings.PurchaseWebhookUrl);
                using var client = new RestClient(options);
                var restRequest = new RestRequest()
                    .AddHeader("x-backend-secret", _settings.BackendSecretKey)
                    .AddHeader("Content-Type", "application/json")
                    .AddStringBody(jsonString, DataFormat.Json);

                await client.PostAsync(restRequest, cancellationToken);
                return true;
            }
            catch (Exception)
            {
                // La analítica nunca debe romper la verificación del carrito.
                return false;
            }
        }
    }
}
