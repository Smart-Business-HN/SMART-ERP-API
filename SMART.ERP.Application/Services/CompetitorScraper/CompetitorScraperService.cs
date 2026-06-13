using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using RestSharp;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.ERP.Domain.Settings;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Services.CompetitorScraper
{
    /// <summary>
    /// Lee precios de competencia. Sycom (Odoo) sirve el precio en el HTML → HtmlCssSelector/JsonLd.
    /// Acosa (WooCommerce) lo renderiza con JS → Headless (Playwright). Nunca lanza hacia afuera.
    /// </summary>
    public class CompetitorScraperService : ICompetitorScraperService
    {
        private static readonly Regex PriceTokenRegex = new(@"[\d][\d.,]*", RegexOptions.Compiled);

        private readonly RepricingScraperSettings _settings;
        private readonly ILogger<CompetitorScraperService> _logger;

        public CompetitorScraperService(
            IOptions<RepricingScraperSettings> settings,
            ILogger<CompetitorScraperService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<ScrapeResult> ScrapeAsync(CompetitorSource source, CancellationToken ct = default)
        {
            try
            {
                return source.ParseStrategy switch
                {
                    // Manual: el precio lo mantiene el admin; devolvemos el último valor guardado.
                    ParseStrategy.Manual => source.LastObservedPrice.HasValue
                        ? ScrapeResult.Ok(source.LastObservedPrice.Value, source.LastObservedInStock)
                        : ScrapeResult.Fail("Fuente manual sin precio ingresado."),
                    ParseStrategy.HtmlCssSelector => await ScrapeHtmlAsync(source, ct),
                    ParseStrategy.JsonLd => await ScrapeJsonLdAsync(source, ct),
                    ParseStrategy.Headless => await ScrapeHeadlessAsync(source, ct),
                    _ => ScrapeResult.Fail($"Estrategia no soportada: {source.ParseStrategy}")
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fallo al leer competidor {Competitor} para producto {ProductId}",
                    source.CompetitorName, source.ProductId);
                return ScrapeResult.Fail(ex.Message);
            }
        }

        private async Task<string?> HttpGetAsync(string url, CancellationToken ct)
        {
            var options = new RestClientOptions(url)
            {
                Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds),
                UserAgent = _settings.UserAgent,
                FollowRedirects = true
            };
            using var client = new RestClient(options);
            var response = await client.GetAsync(new RestRequest(), ct);
            return response.IsSuccessful ? response.Content : null;
        }

        private async Task<ScrapeResult> ScrapeHtmlAsync(CompetitorSource source, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(source.PriceSelector))
                return ScrapeResult.Fail("Selector CSS no configurado.");

            var html = await HttpGetAsync(source.ProductUrl, ct);
            if (string.IsNullOrEmpty(html))
                return ScrapeResult.Fail("La página no devolvió contenido (¿bloqueo o JS requerido?).");

            using var context = BrowsingContext.New(Configuration.Default);
            using var doc = await context.OpenAsync(req => req.Content(html), ct);

            var element = doc.QuerySelector(source.PriceSelector);
            if (element is null)
                return ScrapeResult.Fail($"No se encontró el selector '{source.PriceSelector}'.");

            if (!TryParsePrice(element.TextContent, out var price))
                return ScrapeResult.Fail($"No se pudo interpretar el precio: '{element.TextContent?.Trim()}'.");

            // InStock = null (desconocido → se considera disponible). No usamos heurística de texto del body
            // ("agotado"/"no disponible") porque es poco fiable y descartaría precios que sí queremos igualar.
            // La disponibilidad explícita solo se respeta cuando viene en JSON-LD (offers.availability).
            return ScrapeResult.Ok(price, null);
        }

        private async Task<ScrapeResult> ScrapeJsonLdAsync(CompetitorSource source, CancellationToken ct)
        {
            var html = await HttpGetAsync(source.ProductUrl, ct);
            if (string.IsNullOrEmpty(html))
                return ScrapeResult.Fail("La página no devolvió contenido (¿bloqueo o JS requerido?).");

            using var context = BrowsingContext.New(Configuration.Default);
            using var doc = await context.OpenAsync(req => req.Content(html), ct);

            foreach (var script in doc.QuerySelectorAll("script[type='application/ld+json']"))
            {
                if (TryReadJsonLdOffer(script.TextContent, out var price, out var inStock))
                    return ScrapeResult.Ok(price, inStock);
            }
            return ScrapeResult.Fail("No se encontró un bloque JSON-LD con offers.price.");
        }

        private async Task<ScrapeResult> ScrapeHeadlessAsync(CompetitorSource source, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(source.PriceSelector))
                return ScrapeResult.Fail("Selector CSS no configurado.");

            // Nota: requiere los navegadores de Playwright instalados en el host (`playwright install chromium`).
            // Si no están, CreateAsync/LaunchAsync lanza y lo capturamos como fallo (no rompe la corrida).
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync(new BrowserNewPageOptions { UserAgent = _settings.UserAgent });

            await page.GotoAsync(source.ProductUrl, new PageGotoOptions
            {
                Timeout = _settings.TimeoutSeconds * 1000,
                WaitUntil = WaitUntilState.NetworkIdle
            });

            try
            {
                await page.WaitForSelectorAsync(source.PriceSelector,
                    new PageWaitForSelectorOptions { Timeout = _settings.TimeoutSeconds * 1000 });
            }
            catch (TimeoutException)
            {
                return ScrapeResult.Fail($"El selector '{source.PriceSelector}' no apareció a tiempo.");
            }

            var text = await page.InnerTextAsync(source.PriceSelector);
            if (!TryParsePrice(text, out var price))
                return ScrapeResult.Fail($"No se pudo interpretar el precio: '{text?.Trim()}'.");

            return ScrapeResult.Ok(price, null);
        }

        private static bool TryReadJsonLdOffer(string? json, out decimal price, out bool? inStock)
        {
            price = 0m;
            inStock = null;
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                using var document = JsonDocument.Parse(json);
                return FindOffer(document.RootElement, ref price, ref inStock);
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private static bool FindOffer(JsonElement element, ref decimal price, ref bool? inStock)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Array:
                    foreach (var item in element.EnumerateArray())
                        if (FindOffer(item, ref price, ref inStock)) return true;
                    return false;
                case JsonValueKind.Object:
                    if (element.TryGetProperty("offers", out var offers))
                    {
                        if (FindOffer(offers, ref price, ref inStock)) return true;
                    }
                    if (element.TryGetProperty("price", out var priceProp) &&
                        TryReadJsonNumber(priceProp, out price))
                    {
                        if (element.TryGetProperty("availability", out var avail) &&
                            avail.ValueKind == JsonValueKind.String)
                        {
                            var a = avail.GetString() ?? string.Empty;
                            inStock = !a.Contains("OutOfStock", StringComparison.OrdinalIgnoreCase) &&
                                      !a.Contains("SoldOut", StringComparison.OrdinalIgnoreCase);
                        }
                        return true;
                    }
                    // Recorrer hijos por si el offer está anidado.
                    foreach (var child in element.EnumerateObject())
                        if ((child.Value.ValueKind == JsonValueKind.Object || child.Value.ValueKind == JsonValueKind.Array)
                            && FindOffer(child.Value, ref price, ref inStock)) return true;
                    return false;
                default:
                    return false;
            }
        }

        private static bool TryReadJsonNumber(JsonElement element, out decimal value)
        {
            value = 0m;
            if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out value))
                return true;
            if (element.ValueKind == JsonValueKind.String &&
                TryParsePrice(element.GetString(), out value))
                return true;
            return false;
        }

        /// <summary>
        /// Interpreta un precio en formato es-HN ("L 7,595.00", "7595.0 HNL"): coma = miles, punto = decimal.
        /// </summary>
        internal static bool TryParsePrice(string? text, out decimal price)
        {
            price = 0m;
            if (string.IsNullOrWhiteSpace(text)) return false;

            var match = PriceTokenRegex.Match(text);
            if (!match.Success) return false;

            var token = match.Value.Replace(",", string.Empty); // quitar separador de miles
            return decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out price) && price > 0m;
        }
    }
}
