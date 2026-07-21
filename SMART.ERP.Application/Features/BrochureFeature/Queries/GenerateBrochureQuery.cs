using MediatR;
using SMART.ERP.Application.DTOs.Brochure;
using SMART.ERP.Application.Services.BrochureDataService;
using SMART.ERP.Application.Services.BrochurePdfService;

namespace SMART.ERP.Application.Features.BrochureFeature.Queries
{
    /// <summary>
    /// Genera el PDF. Devuelve bytes crudos (sin envolver en <c>Response&lt;T&gt;</c>),
    /// igual que <c>GenerateProductKardexPdfQuery</c>.
    /// </summary>
    public class GenerateBrochureQuery : BrochureFilterDto, IRequest<byte[]>
    {
        public class GenerateBrochureQueryHandler : IRequestHandler<GenerateBrochureQuery, byte[]>
        {
            private readonly IBrochureDataService _dataService;
            private readonly IBrochurePdfService _pdfService;

            public GenerateBrochureQueryHandler(
                IBrochureDataService dataService,
                IBrochurePdfService pdfService)
            {
                _dataService = dataService;
                _pdfService = pdfService;
            }

            public async Task<byte[]> Handle(GenerateBrochureQuery request, CancellationToken cancellationToken)
            {
                // BuildDocumentAsync revalida por su cuenta (tope, precios, lista activa):
                // no se confía en que el cliente haya llamado antes a Preview.
                var document = await _dataService.BuildDocumentAsync(request, cancellationToken);
                return await _pdfService.GeneratePdfAsync(document);
            }
        }
    }
}
