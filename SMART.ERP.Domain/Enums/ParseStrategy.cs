namespace SMART.ERP.Domain.Enums
{
    /// <summary>
    /// Cómo se lee el precio de una fuente de competencia (<see cref="Entities.CompetitorSource"/>).
    /// </summary>
    public enum ParseStrategy
    {
        /// <summary>HTTP GET + parseo de un selector CSS sobre el HTML server-rendered (ej. Sycom/Odoo).</summary>
        HtmlCssSelector = 1,
        /// <summary>HTTP GET + lectura de offers.price desde un bloque JSON-LD (<c>application/ld+json</c>).</summary>
        JsonLd = 2,
        /// <summary>Navegador headless (precio renderizado por JavaScript, ej. Acosa/WooCommerce con loader).</summary>
        Headless = 3,
        /// <summary>Sin scraping: el precio lo ingresa el administrador a mano.</summary>
        Manual = 4
    }
}
