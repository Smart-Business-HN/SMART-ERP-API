using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Búsqueda de productos centralizada (portada de VENTIX) y compartida por admin y ecommerce.
    /// - Tokeniza el término por espacios: cada token DEBE coincidir (AND) en al menos un campo (OR).
    /// - Ordena por un score de relevancia de varios niveles (nombre, código/SKU, marca, categoría, descripción).
    /// - Comparación insensible a mayúsculas Y acentos vía collation SQL Server (datos en español de Honduras).
    /// </summary>
    public static class ProductSearchPredicate
    {
        /// <summary>
        /// Pareja accent-insensitive de la collation por defecto del servidor (SQL_Latin1_General_CP1_CI_AS).
        /// Garantiza que "café" == "cafe" en las comparaciones. Debe inlinarse como literal en las
        /// expresiones (EF.Functions.Collate exige una collation constante), por eso es const.
        /// </summary>
        public const string Collation = "Latin1_General_CI_AI";

        public static string[] Tokenize(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Array.Empty<string>();
            }
            return search.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Filtro: AND entre tokens, OR entre campos buscables. <paramref name="useEcommerceFields"/>
        /// añade EcommerceDescription a los campos del cliente final.
        /// </summary>
        public static void Apply(ISpecificationBuilder<Product> query, string? search, bool useEcommerceFields)
        {
            var tokens = Tokenize(search);
            if (tokens.Length == 0)
            {
                return;
            }

            foreach (var token in tokens)
            {
                var t = token;
                query.Where(p =>
                    EF.Functions.Collate(p.Name, Collation).Contains(t)
                    || EF.Functions.Collate(p.Code, Collation).Contains(t)
                    || (p.Description != null && EF.Functions.Collate(p.Description, Collation).Contains(t))
                    || (useEcommerceFields && p.EcommerceDescription != null && EF.Functions.Collate(p.EcommerceDescription, Collation).Contains(t))
                    || (p.Brand != null && EF.Functions.Collate(p.Brand.Name, Collation).Contains(t))
                    || (p.SubCategory != null && EF.Functions.Collate(p.SubCategory.Name, Collation).Contains(t))
                    || (p.SubCategory != null && p.SubCategory.Category != null && EF.Functions.Collate(p.SubCategory.Category.Name, Collation).Contains(t))
                    || (p.ProductSubcategories != null && p.ProductSubcategories.Any(ps =>
                        ps.Subcategory != null && EF.Functions.Collate(ps.Subcategory.Name, Collation).Contains(t))));
            }
        }

        /// <summary>
        /// Ordenamiento. Precedencia:
        /// 1) <paramref name="sortBy"/> explícito (price/price_asc/price_desc/newest/name).
        /// 2) <paramref name="legacyColumn"/> (orden por columna del admin) salvo que se pida "relevance".
        /// 3) Score de relevancia cuando hay tokens.
        /// 4) Por nombre.
        /// </summary>
        public static void ApplyOrdering(
            ISpecificationBuilder<Product> builder,
            string? search,
            string? sortBy = null,
            bool useEcommerceFields = false,
            string? legacyOrder = null,
            string? legacyColumn = null)
        {
            var sort = (sortBy ?? string.Empty).Trim().ToLowerInvariant();

            // 1) Orden explícito no-relevancia: siempre gana (incluso con búsqueda activa).
            switch (sort)
            {
                case "price":
                case "price_asc":
                    builder.OrderBy(p => p.RecomendedSalePrice).ThenBy(p => p.Name);
                    return;
                case "price_desc":
                    builder.OrderByDescending(p => p.RecomendedSalePrice).ThenBy(p => p.Name);
                    return;
                case "newest":
                    builder.OrderByDescending(p => p.CreationDate).ThenBy(p => p.Name);
                    return;
                case "name":
                    builder.OrderBy(p => p.Name);
                    return;
            }

            // 2) Orden por columna del admin (header clickeado). Se ignora si el llamador pidió "relevance"
            //    (defensa en profundidad: el ecommerce manda column='name' fijo).
            if (!string.IsNullOrEmpty(legacyColumn) && sort != "relevance")
            {
                if (string.Equals(legacyOrder, "desc", StringComparison.OrdinalIgnoreCase))
                {
                    builder.OrderByDescending(GetColumnExpression(legacyColumn));
                }
                else
                {
                    builder.OrderBy(GetColumnExpression(legacyColumn));
                }
                return;
            }

            // 3) Relevancia por score (un nivel de ordenamiento por token), con desempate por nombre.
            var tokens = Tokenize(search);
            if (tokens.Length > 0)
            {
                var ordered = builder.OrderByDescending(BuildScoreExpression(tokens[0], useEcommerceFields));
                for (var i = 1; i < tokens.Length; i++)
                {
                    ordered = ordered.ThenByDescending(BuildScoreExpression(tokens[i], useEcommerceFields));
                }
                ordered.ThenBy(p => p.Name);
                return;
            }

            // 4) Sin término ni orden explícito.
            builder.OrderBy(p => p.Name);
        }

        /// <summary>
        /// Score aditivo por token. Los matches de "palabra completa" y de Code exacto/prefijo se evalúan
        /// con EF.Functions.Like sobre la columna collated (100% traducible a SQL) en lugar de concatenar la
        /// columna; los substrings con Collate(...).Contains (auto-escapa el parámetro).
        /// </summary>
        private static Expression<Func<Product, object?>> BuildScoreExpression(string token, bool useEcommerceFields)
        {
            var esc = EscapeLike(token);
            var exact = esc;               // palabra/código idéntico
            var prefix = esc + "%";        // empieza por el token
            var wordStart = esc + " %";    // token al inicio, seguido de espacio
            var wordEnd = "% " + esc;      // token al final, precedido de espacio
            var wordMid = "% " + esc + " %"; // token rodeado de espacios
            var t = token;                 // substring (Contains escapa por sí solo)

            return p =>
                // Code exacto (intención más fuerte en un ERP).
                (EF.Functions.Like(EF.Functions.Collate(p.Code, Collation), exact) ? 1200 : 0)
                // Palabra completa en Name.
                + ((EF.Functions.Like(EF.Functions.Collate(p.Name, Collation), exact)
                    || EF.Functions.Like(EF.Functions.Collate(p.Name, Collation), wordStart)
                    || EF.Functions.Like(EF.Functions.Collate(p.Name, Collation), wordEnd)
                    || EF.Functions.Like(EF.Functions.Collate(p.Name, Collation), wordMid)) ? 1000 : 0)
                // Code por prefijo.
                + (EF.Functions.Like(EF.Functions.Collate(p.Code, Collation), prefix) ? 600 : 0)
                // Palabra completa en Brand.
                + ((p.Brand != null && (
                    EF.Functions.Like(EF.Functions.Collate(p.Brand.Name, Collation), exact)
                    || EF.Functions.Like(EF.Functions.Collate(p.Brand.Name, Collation), wordStart)
                    || EF.Functions.Like(EF.Functions.Collate(p.Brand.Name, Collation), wordEnd)
                    || EF.Functions.Like(EF.Functions.Collate(p.Brand.Name, Collation), wordMid))) ? 500 : 0)
                // Code como substring.
                + (EF.Functions.Collate(p.Code, Collation).Contains(t) ? 300 : 0)
                // Name como substring.
                + (EF.Functions.Collate(p.Name, Collation).Contains(t) ? 100 : 0)
                // Brand como substring.
                + ((p.Brand != null && EF.Functions.Collate(p.Brand.Name, Collation).Contains(t)) ? 50 : 0)
                // Categoría / subcategoría canónica (la principal y su categoría). Nota: el match por
                // subcategorías SECUNDARIAS (ProductSubcategories) se hace en Apply/WHERE; NO se incluye
                // aquí porque EF no traduce un subquery .Any() en disyunción con navegaciones dentro del ORDER BY.
                + (((p.SubCategory != null && EF.Functions.Collate(p.SubCategory.Name, Collation).Contains(t))
                    || (p.SubCategory != null && p.SubCategory.Category != null && EF.Functions.Collate(p.SubCategory.Category.Name, Collation).Contains(t))) ? 30 : 0)
                // Descripción.
                + (((p.Description != null && EF.Functions.Collate(p.Description, Collation).Contains(t))
                    || (useEcommerceFields && p.EcommerceDescription != null && EF.Functions.Collate(p.EcommerceDescription, Collation).Contains(t))) ? 5 : 0);
        }

        /// <summary>Mapea el nombre de columna (admin v1 mayúsculas o v2 minúsculas) a una expresión de orden.</summary>
        private static Expression<Func<Product, object?>> GetColumnExpression(string column)
        {
            return column.ToLowerInvariant() switch
            {
                "name" => x => x.Name,
                "code" => x => x.Code,
                "price" => x => x.RecomendedSalePrice,
                "brand" => x => x.Brand!.Name,
                "category" => x => x.SubCategory!.Category!.Name,
                "subcategory" => x => x.SubCategory!.Name,
                "stock" => x => x.CurrentStock,
                "createdate" => x => x.CreationDate,
                _ => x => x.Name
            };
        }

        /// <summary>Escapa los comodines LIKE de SQL Server usando clases de caracteres (sin cláusula ESCAPE).</summary>
        private static string EscapeLike(string input)
        {
            return input
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
        }
    }
}
