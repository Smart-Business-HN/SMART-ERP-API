using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.QuotationFeature.Commands.DownloadPDFQuotationCommand
{
    public class InvoiceDocument : IDocument
    {
        public QuotationDto Model { get; }

        public InvoiceDocument(QuotationDto model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Height(100).Background(Colors.Grey.Lighten1);
                    page.Content().Background(Colors.Grey.Lighten3);
                    page.Footer().Height(50).Background(Colors.Grey.Lighten1);
                });
        }
    }
    public interface IDocument
    {
        DocumentMetadata GetMetadata();
        DocumentSettings GetSettings();
        void Compose(IDocumentContainer container);
    }





}
