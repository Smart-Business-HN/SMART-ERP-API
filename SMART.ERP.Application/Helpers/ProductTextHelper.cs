using System.Net;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Helpers
{
    /// <summary>
    /// Convierte los campos de descripción de un producto en texto plano apto para
    /// una tarjeta de brochure.
    /// </summary>
    public static partial class ProductTextHelper
    {
        /// <summary>Largo que cabe en una tarjeta del grid 2x3 sin desbordarla.</summary>
        public const int BrochureDescriptionLength = 160;

        [GeneratedRegex(@"<[^>]+>", RegexOptions.Compiled)]
        private static partial Regex HtmlTagRegex();

        [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
        private static partial Regex WhitespaceRegex();

        [GeneratedRegex(@"<(br|/p|/div|/li|/h[1-6]|/tr|/td)\s*/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
        private static partial Regex BlockCloseRegex();

        /// <summary>
        /// Descripción corta para la tarjeta. Prefiere <c>Description</c> (texto plano);
        /// si viene vacía cae a <c>EcommerceDescription</c> (HTML) ya saneada.
        /// </summary>
        public static string BuildShortDescription(
            string? description,
            string? ecommerceDescription,
            int maxLength = BrochureDescriptionLength)
        {
            // Se usa IsNullOrWhiteSpace y NO el operador '??': EcommerceDescription
            // suele llegar como cadena vacía (no null), y '??' no haría el respaldo.
            var source = !string.IsNullOrWhiteSpace(description) ? description : ecommerceDescription;
            return Truncate(StripHtml(source), maxLength);
        }

        /// <summary>HTML → texto plano de una sola línea.</summary>
        public static string StripHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            // 1. Cerrar bloques como espacio ANTES de borrar etiquetas.
            //    Si no, "<p>Cable</p><p>UTP</p>" se convierte en "CableUTP".
            var text = BlockCloseRegex().Replace(html, " ");

            // 2. Quitar el resto de etiquetas.
            text = HtmlTagRegex().Replace(text, string.Empty);

            // 3. Decodificar entidades. Ojo: &nbsp; se decodifica a U+00A0,
            //    que NO es un espacio normal.
            text = WebUtility.HtmlDecode(text);

            // 4. Defecto de datos conocido: ecommerceDescription trae TODOS los
            //    espacios como &nbsp;. Sin este paso el texto queda como un único
            //    token indivisible, no envuelve, y desborda la tarjeta.
            text = text
                .Replace('\u00A0', ' ')           // no-break space (&nbsp;)
                .Replace('\u2007', ' ')           // figure space
                .Replace('\u202F', ' ')           // narrow no-break space
                .Replace("\u200B", string.Empty) // zero-width space
                .Replace("\u00AD", string.Empty); // soft hyphen

            // 5. Colapsar espacios/saltos.
            return WhitespaceRegex().Replace(text, " ").Trim();
        }

        /// <summary>Corta en frontera de palabra y agrega puntos suspensivos.</summary>
        public static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength) return text;

            var slice = text[..maxLength];
            var lastSpace = slice.LastIndexOf(' ');

            // Si no hay espacio, o el corte dejaría un fragmento ridículo, se corta duro.
            if (lastSpace > maxLength / 2) slice = slice[..lastSpace];

            return slice.TrimEnd(' ', ',', ';', '.', '-') + "…";
        }
    }
}
