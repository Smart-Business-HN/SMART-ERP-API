using System.Globalization;

namespace SMART.ERP.Application.Helpers
{
    /// <summary>
    /// Identidad visual de Smart Business para documentos generados (PDF).
    /// Los colores replican los tokens del rediseño del ecommerce, de modo que el
    /// material impreso y la tienda se vean como el mismo sistema.
    ///
    /// Se declara en Application (no en Infrastructure) porque los DTOs y los
    /// handlers también necesitan las constantes de empresa. Los colores son
    /// <see cref="string"/> a propósito: Application no referencia QuestPDF, y
    /// QuestPDF convierte implícitamente de string a su tipo Color.
    /// </summary>
    public static class BrandKit
    {
        // ── Paleta ────────────────────────────────────────────────────────────
        public const string Ink = "#0A0D14";
        public const string InkSoft = "#14171C";
        public const string Blue = "#006FFF";
        public const string BlueSoft = "#5AA0FF";
        public const string Grey = "#79808B";
        public const string GreyLight = "#9AA1AC";
        public const string Surface = "#F6F7F8";
        public const string Border = "#ECEEF1";
        public const string Placeholder = "#C9CED6";
        public const string White = "#FFFFFF";
        public const string Green = "#39E0A0";

        // ── Datos de la empresa ───────────────────────────────────────────────
        // Levantados de QuotationPdfService.ComposeContent, donde estaban embebidos.
        public const string CompanyName = "SMART BUSINESS S. DE R.L.";
        public const string CompanyAddress = "Bo. Barandillas 9 cll 7 y 8 ave. Edif. Robles 2da Planta.\nSan Pedro Sula, Cortes.";
        public const string CompanyRtn = "01019021333211";
        public const string CompanyPhones = "(+504) 8734-1687 / (+504) 8818-7765";
        public const string CompanyEmail = "ventas@smartbusiness.site";
        public const string CompanyWebsite = "https://www.smartbusiness.site";

        // ── Assets ────────────────────────────────────────────────────────────
        // Viven en SMART.ERP.API/Assets y se copian al output vía el glob
        // <Content Include="Assets\**"> del csproj. Se resuelven con
        // ContentRootPath: este proyecto NO tiene wwwroot.
        public const string AssetsFolder = "Assets";
        public const string LogoDark = "company-image.png";
        public const string LogoWhite = "smart-business-logo-white.png";
        public const string IconFacebook = "facebook.png";
        public const string IconInstagram = "instagram.png";
        public const string IconWhatsapp = "whatsapp.png";

        // ── Formato ───────────────────────────────────────────────────────────
        private static readonly CultureInfo HnCulture = CultureInfo.GetCultureInfo("es-HN");

        public static string FormatCurrency(decimal amount) => $"L. {amount:N2}";

        /// <summary>"Julio 2026" — para la portada.</summary>
        public static string SpanishMonthYear(DateTime date)
        {
            var month = HnCulture.DateTimeFormat.GetMonthName(date.Month);
            return $"{char.ToUpper(month[0], HnCulture)}{month[1..]} {date.Year}";
        }

        /// <summary>"20/07/2026" — para la nota de vigencia de la contraportada.</summary>
        public static string FormatDate(DateTime date) => date.ToString("dd/MM/yyyy", HnCulture);
    }
}
