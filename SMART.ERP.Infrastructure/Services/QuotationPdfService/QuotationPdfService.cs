using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Services.QuotationPdfService;

namespace SMART.ERP.Infrastructure.Services.QuotationPdfService
{
    public class QuotationPdfService : IQuotationPdfService
    {
        private readonly IWebHostEnvironment _environment;

        public QuotationPdfService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public Task<byte[]> GeneratePdfAsync(QuotationPreviewDto quotation)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.MarginHorizontal(40);
                    page.MarginVertical(30);
                    page.DefaultTextStyle(x => x.FontSize(9).FontColor("#555555"));

                    page.Header().Element(c => ComposeHeader(c, quotation));
                    page.Content().Element(c => ComposeContent(c, quotation));
                    page.Footer().Element(c => ComposeFooter(c));
                });
            });

            var pdfBytes = document.GeneratePdf();
            return Task.FromResult(pdfBytes);
        }

        private void ComposeHeader(IContainer container, QuotationPreviewDto quotation)
        {
            container.Column(column =>
            {
                // Logo
                var logoPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "images", "smart_business_logo.png");
                if (File.Exists(logoPath))
                {
                    column.Item().Height(28).Width(120).Image(logoPath);
                }
                else
                {
                    column.Item().Text("SMART BUSINESS S. DE R.L.").Bold().FontSize(14).FontColor("#1E293B");
                }

                column.Item().PaddingVertical(4).LineHorizontal(1).LineColor("#000000");
            });
        }

        private void ComposeContent(IContainer container, QuotationPreviewDto quotation)
        {
            var creationDate = quotation.CreationDate.ToShortDateString();
            var dueDate = quotation.DueDate.ToShortDateString();

            // Calculate tax breakdown
            decimal excento = 0, taxable15 = 0, taxable18 = 0;
            if (quotation.ProductsOffered != null)
            {
                foreach (var p in quotation.ProductsOffered)
                {
                    var lineSubtotal = p.Quantity * p.UnitPrice;
                    if (p.TaxId == 1) taxable15 += lineSubtotal;
                    else if (p.TaxId == 3) taxable18 += lineSubtotal;
                    else if (p.TaxId == 2) excento += lineSubtotal;
                }
            }
            decimal isv15 = taxable15 * 0.15m;
            decimal isv18 = taxable18 * 0.18m;
            decimal total = taxable15 + taxable18 + isv15 + isv18 + excento;

            container.Column(column =>
            {
                column.Spacing(6);

                // Quotation info + Company info
                column.Item().Row(row =>
                {
                    row.RelativeItem(3).Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("COTIZACION: ").Bold().FontSize(17).FontColor("#555555");
                            text.Span(quotation.QuotationCode ?? "").Bold().FontSize(17).FontColor("#1E293B");
                        });
                        col.Item().Row(infoRow =>
                        {
                            infoRow.ConstantItem(110).Column(labels =>
                            {
                                labels.Item().Text("Fecha de emisión:").Bold().FontSize(11).FontColor("#1E293B");
                                labels.Item().Text("Válida hasta:").Bold().FontSize(11).FontColor("#1E293B");
                                labels.Item().Text("Comercial:").Bold().FontSize(11).FontColor("#1E293B");
                            });
                            infoRow.RelativeItem().Column(values =>
                            {
                                values.Item().Text(creationDate).FontSize(11).FontColor("#555555");
                                values.Item().Text(dueDate).FontSize(11).FontColor("#555555");
                                values.Item().Text(quotation.UserFullName ?? "N/A").FontSize(11).FontColor("#555555");
                            });
                        });
                    });

                    row.ConstantItem(195).Column(col =>
                    {
                        col.Item().Text("SMART BUSINESS S. DE R.L.").Bold().FontSize(14).FontColor("#1E293B");
                        col.Item().Text("Bo. Barandillas 9 cll 7 y 8 ave. Edif. Robles 2da Planta.\nSan Pedro Sula, Cortes.").Bold().FontSize(8).FontColor("#555555");
                        col.Item().Text(text =>
                        {
                            text.Span("RTN: ").Bold().FontSize(8).FontColor("#1E293B");
                            text.Span("01019021333211").Bold().FontSize(8).FontColor("#555555");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Tel: ").Bold().FontSize(8).FontColor("#1E293B");
                            text.Span("(+504) 8734-1687 / (+504) 8818-7765").Bold().FontSize(8).FontColor("#555555");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Email: ").Bold().FontSize(8).FontColor("#1E293B");
                            text.Span("ventas@smartbusiness.site").Bold().FontSize(8).FontColor("#555555");
                        });
                    });
                });

                // Customer info
                column.Item().Row(row =>
                {
                    row.RelativeItem(3).Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("CLIENTE: ").Bold().FontSize(10).FontColor("#1E293B");
                            text.Span((quotation.CustomerName ?? "N/A").ToUpper()).Bold().FontSize(10).FontColor("#555555");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Dirección: ").Bold().FontSize(10).FontColor("#1E293B");
                            text.Span(quotation.CustomerAddress ?? "N/A").Bold().FontSize(10).FontColor("#555555");
                        });
                    });
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().Text(text =>
                        {
                            text.Span("RTN: ").Bold().FontSize(10).FontColor("#1E293B");
                            text.Span(quotation.CustomerRtn ?? "N/A").Bold().FontSize(10).FontColor("#555555");
                        });
                        col.Item().Text(text =>
                        {
                            text.Span("Tel: ").Bold().FontSize(10).FontColor("#1E293B");
                            text.Span(quotation.CustomerPhone ?? "N/A").Bold().FontSize(10).FontColor("#555555");
                        });
                    });
                });

                // Products table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();   // Description
                        columns.ConstantColumn(30);  // Qty
                        columns.ConstantColumn(80);  // Unit Price
                        columns.ConstantColumn(25);  // ISV
                        columns.ConstantColumn(75);  // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().BorderBottom(1).BorderColor("#999999").Padding(4)
                            .Text("Descripción").Bold().FontSize(11).FontColor("#1E293B");
                        header.Cell().BorderBottom(1).BorderColor("#999999").Padding(4)
                            .AlignCenter().Text("Cant.").Bold().FontSize(11).FontColor("#1E293B");
                        header.Cell().BorderBottom(1).BorderColor("#999999").Padding(4)
                            .AlignCenter().Text("Precio Unitario").Bold().FontSize(11).FontColor("#1E293B");
                        header.Cell().BorderBottom(1).BorderColor("#999999").Padding(4)
                            .AlignCenter().Text("ISV").Bold().FontSize(11).FontColor("#1E293B");
                        header.Cell().BorderBottom(1).BorderColor("#999999").Padding(4)
                            .AlignCenter().Text("Total").Bold().FontSize(11).FontColor("#1E293B");
                    });

                    if (quotation.ProductsOffered != null)
                    {
                        foreach (var product in quotation.ProductsOffered)
                        {
                            var description = $"[{product.ProductCode}] {product.ProductDescription}";
                            var lineTotal = product.Quantity * product.UnitPrice;

                            table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(3)
                                .Text(description).FontSize(9).FontColor("#555555");
                            table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(3)
                                .AlignCenter().Text(product.Quantity.ToString("0")).FontSize(9).FontColor("#555555");
                            table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(3)
                                .Text(FormatCurrency(product.UnitPrice)).FontSize(9).FontColor("#555555");
                            table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(3)
                                .AlignCenter().Text($"{product.TaxRate}%").FontSize(9).FontColor("#555555");
                            table.Cell().BorderBottom(0.5f).BorderColor("#DDDDDD").Padding(3)
                                .AlignRight().Text(FormatCurrency(lineTotal)).FontSize(9).FontColor("#555555");
                        }
                    }
                });

                column.Item().PaddingTop(8);

                // Summary section
                column.Item().Row(row =>
                {
                    // Left: Observations & Terms
                    row.RelativeItem(3).Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#000000");
                        col.Item().PaddingTop(4);

                        if (!string.IsNullOrEmpty(quotation.Observations))
                        {
                            col.Item().Text("Observaciones:").Bold().FontSize(10).FontColor("#1E293B");
                            col.Item().Text(quotation.Observations).Bold().FontSize(10).FontColor("#555555");
                            col.Item().PaddingTop(4);
                        }

                        if (!string.IsNullOrEmpty(quotation.TermsAndConditions))
                        {
                            col.Item().Text("Términos y condiciones:").Bold().FontSize(10).FontColor("#1E293B");
                            col.Item().Text(quotation.TermsAndConditions).Bold().FontSize(10).FontColor("#555555");
                        }
                    });

                    // Right: Tax breakdown
                    row.ConstantItem(200).Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#000000");
                        col.Item().PaddingTop(4);

                        AddSummaryRow(col, "Exento:", FormatCurrency(excento));
                        AddSummaryRow(col, "Exonerado:", "L. 0.00");
                        AddSummaryRow(col, "Gravado al 15%:", FormatCurrency(taxable15));
                        AddSummaryRow(col, "Gravado al 18%:", FormatCurrency(taxable18));
                        AddSummaryRow(col, "ISV 15%:", FormatCurrency(isv15));
                        AddSummaryRow(col, "ISV 18%:", FormatCurrency(isv18));

                        col.Item().PaddingTop(4).Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text("TOTAL:").Bold().FontSize(12).FontColor("#1E293B");
                            r.ConstantItem(90).AlignRight().Text(FormatCurrency(total)).Bold().FontSize(12).FontColor("#555555");
                        });
                    });
                });
            });
        }

        private static void AddSummaryRow(ColumnDescriptor col, string label, string value)
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().AlignRight().Text(label).Bold().FontSize(10).FontColor("#1E293B");
                r.ConstantItem(90).AlignRight().Text(value).Bold().FontSize(10).FontColor("#555555");
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.BorderTop(1).BorderColor("#999999").PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text("https://www.smartbusiness.site").FontSize(10).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber().FontSize(10);
                    text.Span(" de ").FontSize(10);
                    text.TotalPages().FontSize(10);
                });
                row.RelativeItem().AlignRight().Text("SMART BUSINESS S. DE R.L.").FontSize(10).FontColor(Colors.Grey.Medium);
            });
        }

        private static string FormatCurrency(decimal amount)
        {
            return $"L. {amount:N2}";
        }
    }
}
