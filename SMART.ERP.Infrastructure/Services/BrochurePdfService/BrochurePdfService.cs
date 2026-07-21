using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMART.ERP.Application.DTOs.Brochure;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Services.BrochurePdfService;

namespace SMART.ERP.Infrastructure.Services.BrochurePdfService
{
    /// <summary>
    /// Brochure comercial: portada compuesta, rejilla 2x3 de productos agrupada por
    /// categoría, y contraportada con datos de contacto.
    /// </summary>
    public class BrochurePdfService : IBrochurePdfService
    {
        // ── Geometría de la tarjeta ───────────────────────────────────────────
        // La altura es FIJA para que la rejilla no se desalinee cuando un producto
        // tiene nombre largo y el de al lado no. Height() lanza DocumentLayoutException
        // si el contenido desborda, así que todo texto variable va acotado con
        // ClampLines y los bloques internos suman exactamente CardInnerHeight.
        // Presupuesto vertical de una página de contenido (Carta = 792 pt):
        //   792 - 52 (márgenes) - ~40 (encabezado) - ~18 (pie) - 20 (padding) ≈ 662 pt
        // Para que entren las 3 filas del grid 2x3 incluso en una página que arranca
        // con banda de categoría:
        //   banda(27) + gap(10) + 3 x 200 + 2 gaps(20) = 657 ≤ 662
        private const float CardHeight = 200f;
        private const float CardPadding = 10f;
        private const float ImageHeight = 90f;
        private const float CardGap = 10f;

        // Bloque de texto de alto FIJO para que el precio quede a la misma altura en
        // todas las tarjetas aunque el nombre ocupe una línea o dos.
        //
        // No se usa Extend() como separador: en QuestPDF, Extend() reclama TODO el
        // espacio disponible y desplaza fuera del contenedor a los elementos que le
        // siguen — el precio desaparecía de la tarjeta y el pie se iba a una página
        // huérfana. Alturas fijas + ClampLines es la forma segura.
        //
        // Peor caso del bloque: SKU (~9) + 2 + nombre 2 líneas (10 x 1.15 x 2 = 23)
        // + 3 + descripción 2 líneas (7.5 x 1.2 x 2 = 18) = ~55 pt. Se reservan 58.
        //
        // Reparto interno de la tarjeta (200 - 20 de padding = 180 disponibles):
        //   imagen 90 + 7 + texto 58 + 4 + precio ~18 = 177
        private const float CardTextHeight = 58f;

        private const int ProductsPerRow = 2;

        private readonly IWebHostEnvironment _environment;

        public BrochurePdfService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public Task<byte[]> GeneratePdfAsync(BrochureDocumentDto model)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                ComposeCover(container, model);
                ComposeContentPages(container, model);
                ComposeBackCover(container, model);
            });

            // Refuerzo a nivel documento: las imágenes ya vienen reescaladas y en JPEG,
            // esto solo evita que un asset local suelte el tamaño del archivo.
            document.WithSettings(new DocumentSettings
            {
                ImageCompressionQuality = ImageCompressionQuality.High,
                ImageRasterDpi = 144,
                CompressDocument = true
            });

            return Task.FromResult(document.GeneratePdf());
        }

        // ══ PORTADA ═══════════════════════════════════════════════════════════
        private void ComposeCover(IDocumentContainer container, BrochureDocumentDto model)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(0);
                // Fondo a sangre completa: se pinta la página entera, no un contenedor.
                page.PageColor(BrandKit.Ink);
                page.DefaultTextStyle(x => x.FontColor(BrandKit.White));

                page.Content().Padding(54).Column(col =>
                {
                    var logo = AssetPath(BrandKit.LogoWhite);
                    if (File.Exists(logo))
                    {
                        col.Item().Width(190).Image(logo);
                    }
                    else
                    {
                        col.Item().Text(BrandKit.CompanyName).Bold().FontSize(18);
                    }

                    col.Item().PaddingTop(150).Text(model.Title)
                        .FontSize(38).Bold().LineHeight(1.05f).FontColor(BrandKit.White);

                    // Regla azul de acento.
                    col.Item().PaddingTop(18).Width(96).Height(4).Background(BrandKit.Blue);

                    if (!string.IsNullOrWhiteSpace(model.BrandsLabel))
                    {
                        col.Item().PaddingTop(22).Text(model.BrandsLabel)
                            .FontSize(15).SemiBold().FontColor(BrandKit.BlueSoft);
                    }

                    if (!string.IsNullOrWhiteSpace(model.CategoriesLabel))
                    {
                        col.Item().PaddingTop(4).Text(model.CategoriesLabel)
                            .FontSize(13).FontColor(BrandKit.GreyLight);
                    }
                });

                // El bloque inferior va en Footer, no en Content con un Extend de
                // separador: Extend expulsaría este contenido a una página huérfana.
                page.Footer().PaddingHorizontal(54).PaddingBottom(54).Column(col =>
                {
                    col.Item().Text(BrandKit.SpanishMonthYear(model.GeneratedAt))
                        .FontSize(12).FontColor(BrandKit.GreyLight);
                    col.Item().PaddingTop(2).Text($"Lista de precios: {model.PriceListName}")
                        .FontSize(11).FontColor(BrandKit.Grey);
                    col.Item().PaddingTop(10).Text(BrandKit.CompanyWebsite)
                        .FontSize(11).FontColor(BrandKit.BlueSoft);
                });
            });
        }

        // ══ PÁGINAS DE PRODUCTOS ══════════════════════════════════════════════
        private void ComposeContentPages(IDocumentContainer container, BrochureDocumentDto model)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.MarginHorizontal(34);
                page.MarginVertical(26);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(BrandKit.InkSoft));

                page.Header().Element(c => ComposeHeader(c, model));
                page.Content().PaddingVertical(10).Element(c => ComposeGrid(c, model));
                page.Footer().Element(ComposeFooter);
            });
        }

        private void ComposeHeader(IContainer container, BrochureDocumentDto model)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    var logo = AssetPath(BrandKit.LogoDark);
                    if (File.Exists(logo))
                    {
                        row.ConstantItem(118).AlignMiddle().Image(logo);
                    }
                    else
                    {
                        row.RelativeItem().AlignMiddle().Text(BrandKit.CompanyName)
                            .Bold().FontSize(12).FontColor(BrandKit.Ink);
                    }

                    row.RelativeItem().AlignRight().AlignMiddle().Text(text =>
                    {
                        text.Span(model.PriceListName).SemiBold().FontSize(9).FontColor(BrandKit.Grey);
                        text.Span("  ·  ").FontSize(9).FontColor(BrandKit.Placeholder);
                        text.Span(BrandKit.SpanishMonthYear(model.GeneratedAt))
                            .FontSize(9).FontColor(BrandKit.Grey);
                    });
                });

                col.Item().PaddingTop(8).Height(1).Background(BrandKit.Border);
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.PaddingTop(8).Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text(BrandKit.CompanyWebsite)
                    .FontSize(8).FontColor(BrandKit.GreyLight);

                row.RelativeItem().AlignCenter().AlignMiddle().Text(text =>
                {
                    // Numeración por sección: si se usara CurrentPageNumber() global,
                    // la portada contaría como página 1.
                    text.PageNumberWithinSection("productos").FontSize(8).FontColor(BrandKit.Grey);
                    text.Span(" / ").FontSize(8).FontColor(BrandKit.Placeholder);
                    text.TotalPagesWithinSection("productos").FontSize(8).FontColor(BrandKit.Grey);
                });

                row.RelativeItem().AlignRight().AlignMiddle().Text("Precios con ISV incluido")
                    .FontSize(8).FontColor(BrandKit.GreyLight);
            });
        }

        private void ComposeGrid(IContainer container, BrochureDocumentDto model)
        {
            container.Section("productos").Column(col =>
            {
                col.Spacing(CardGap);

                // Agrupado por categoría, respetando el orden que trajo la consulta.
                var groups = model.Products
                    .GroupBy(p => string.IsNullOrWhiteSpace(p.CategoryName) ? "Otros" : p.CategoryName!)
                    .ToList();

                foreach (var group in groups)
                {
                    col.Item().Element(c => ComposeCategoryBand(c, group.Key));

                    foreach (var chunk in group.Chunk(ProductsPerRow))
                    {
                        col.Item().Height(CardHeight).Row(row =>
                        {
                            row.Spacing(CardGap);

                            foreach (var product in chunk)
                            {
                                row.RelativeItem().Element(c => ComposeCard(c, product));
                            }

                            // Relleno para que una fila impar no estire la última tarjeta
                            // a lo ancho de la página.
                            for (var i = chunk.Length; i < ProductsPerRow; i++)
                            {
                                row.RelativeItem();
                            }
                        });
                    }
                }
            });
        }

        private void ComposeCategoryBand(IContainer container, string categoryName)
        {
            container
                .Background(BrandKit.Ink)
                .CornerRadius(6)
                .PaddingVertical(7)
                .PaddingHorizontal(12)
                .Row(row =>
                {
                    row.ConstantItem(4).Height(13).Background(BrandKit.Blue);
                    row.RelativeItem().PaddingLeft(9).AlignMiddle()
                        .Text(categoryName.ToUpperInvariant())
                        .FontSize(10).Bold().LetterSpacing(0.06f).FontColor(BrandKit.White);
                });
        }

        // ══ TARJETA DE PRODUCTO ═══════════════════════════════════════════════
        private void ComposeCard(IContainer container, BrochureProductItemDto product)
        {
            container
                .Border(1)
                .BorderColor(BrandKit.Border)
                .CornerRadius(10)
                .Background(BrandKit.White)
                .Padding(CardPadding)
                .Column(col =>
                {
                    // ── Imagen ──
                    col.Item().Height(ImageHeight).AlignCenter().AlignMiddle()
                        .Element(c => ComposeCardImage(c, product));

                    // ── Bloque de texto, alto fijo ──
                    col.Item().PaddingTop(7).Height(CardTextHeight).Column(text =>
                    {
                        text.Item().Text(product.Code)
                            .FontSize(7.5f).SemiBold().LetterSpacing(0.05f).FontColor(BrandKit.Blue);

                        text.Item().PaddingTop(2).Text(product.Name)
                            .FontSize(10).SemiBold().LineHeight(1.15f)
                            .FontColor(BrandKit.Ink).ClampLines(2);

                        if (!string.IsNullOrWhiteSpace(product.ShortDescription))
                        {
                            text.Item().PaddingTop(3).Text(product.ShortDescription)
                                .FontSize(7.5f).LineHeight(1.2f)
                                .FontColor(BrandKit.Grey).ClampLines(2);
                        }
                    });

                    // ── Precio ──
                    // Se imprime tal cual viene de la lista: YA INCLUYE ISV.
                    col.Item().PaddingTop(4).Text(BrandKit.FormatCurrency(product.ListPrice))
                        .FontSize(15).Bold().FontColor(BrandKit.Ink);
                });
        }

        private void ComposeCardImage(IContainer container, BrochureProductItemDto product)
        {
            if (product.ImageBytes is { Length: > 0 })
            {
                container.Image(product.ImageBytes).FitArea();
                return;
            }

            // Placeholder: sin assets, solo primitivas, para que nunca falle.
            container
                .Background(BrandKit.Surface)
                .CornerRadius(6)
                .AlignCenter()
                .AlignMiddle()
                .Text("SIN IMAGEN")
                .FontSize(7).SemiBold().LetterSpacing(0.08f).FontColor(BrandKit.Placeholder);
        }

        // ══ CONTRAPORTADA ═════════════════════════════════════════════════════
        private void ComposeBackCover(IDocumentContainer container, BrochureDocumentDto model)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(0);
                page.PageColor(BrandKit.Ink);
                page.DefaultTextStyle(x => x.FontColor(BrandKit.White));

                page.Content().Padding(54).Column(col =>
                {
                    var logo = AssetPath(BrandKit.LogoWhite);
                    if (File.Exists(logo))
                    {
                        col.Item().Width(190).Image(logo);
                    }
                    else
                    {
                        col.Item().Text(BrandKit.CompanyName).Bold().FontSize(18);
                    }

                    col.Item().PaddingTop(46).Text("Contáctanos")
                        .FontSize(24).Bold().FontColor(BrandKit.White);
                    col.Item().PaddingTop(12).Width(96).Height(4).Background(BrandKit.Blue);

                    col.Item().PaddingTop(26).Text(BrandKit.CompanyName)
                        .FontSize(12).Bold().FontColor(BrandKit.White);
                    col.Item().PaddingTop(6).Text(BrandKit.CompanyAddress)
                        .FontSize(10).LineHeight(1.4f).FontColor(BrandKit.GreyLight);

                    col.Item().PaddingTop(12).Element(c => ComposeContactLine(c, "RTN", BrandKit.CompanyRtn));
                    col.Item().PaddingTop(4).Element(c => ComposeContactLine(c, "Tel", BrandKit.CompanyPhones));
                    col.Item().PaddingTop(4).Element(c => ComposeContactLine(c, "Email", BrandKit.CompanyEmail));
                    col.Item().PaddingTop(4).Element(c => ComposeContactLine(c, "Web", BrandKit.CompanyWebsite));

                    col.Item().PaddingTop(28).Element(ComposeSocialIcons);
                });

                page.Footer().PaddingHorizontal(54).PaddingBottom(54).Column(col =>
                {
                    // Procedencia: un brochure que circula meses después debe delatar
                    // su propia antigüedad.
                    col.Item().Text(
                            $"Lista: {model.PriceListName}  ·  Vigente al {BrandKit.FormatDate(model.GeneratedAt)}  ·  Precios sujetos a cambio sin previo aviso.")
                        .FontSize(8).FontColor(BrandKit.Grey);

                    // Si buena parte del catálogo salió sin foto, decirlo aquí: un log
                    // no lo ve quien acaba de descargar el archivo.
                    if (model.Products.Count > 0 &&
                        model.PlaceholderCount * 5 > model.Products.Count)
                    {
                        col.Item().PaddingTop(4).Text(
                                $"{model.PlaceholderCount} de {model.Products.Count} productos aún no tienen fotografía.")
                            .FontSize(8).FontColor(BrandKit.Grey);
                    }
                });
            });
        }

        private void ComposeContactLine(IContainer container, string label, string value)
        {
            container.Row(row =>
            {
                row.ConstantItem(46).Text(label).FontSize(10).SemiBold().FontColor(BrandKit.BlueSoft);
                row.RelativeItem().Text(value).FontSize(10).FontColor(BrandKit.White);
            });
        }

        private void ComposeSocialIcons(IContainer container)
        {
            // Los PNG de Assets son siluetas 100% NEGRAS sobre transparencia: puestos
            // directamente sobre el fondo oscuro serían invisibles. Van montados en un
            // chip blanco redondeado.
            container.Row(row =>
            {
                row.Spacing(10);

                foreach (var icon in new[] { BrandKit.IconFacebook, BrandKit.IconInstagram, BrandKit.IconWhatsapp })
                {
                    var path = AssetPath(icon);
                    if (!File.Exists(path)) continue;

                    row.ConstantItem(28).Height(28)
                        .Background(BrandKit.White)
                        .CornerRadius(14)
                        .Padding(6)
                        .Image(path).FitArea();
                }
            });
        }

        /// <summary>
        /// Los assets se copian al directorio de salida vía el glob
        /// <c>&lt;Content Include="Assets\**"&gt;</c>. Se resuelve contra ContentRootPath
        /// porque este proyecto no tiene wwwroot (WebRootPath es null).
        /// </summary>
        private string AssetPath(string fileName) =>
            Path.Combine(_environment.ContentRootPath, BrandKit.AssetsFolder, fileName);
    }
}
