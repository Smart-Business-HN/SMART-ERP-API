using MediatR;
using SMART.ERP.Application.DTOs.Brochure;
using SMART.ERP.Application.Services.BrochureDataService;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.BrochureFeature.Queries
{
    /// <summary>
    /// Resumen barato de lo que produciría el brochure. Permite al usuario ver cuántos
    /// productos quedan fuera por no tener precio ANTES de disparar una generación
    /// de 15-30 segundos.
    /// </summary>
    public class GetBrochurePreviewQuery : BrochureFilterDto, IRequest<Response<BrochurePreviewDto>>
    {
        public class GetBrochurePreviewQueryHandler
            : IRequestHandler<GetBrochurePreviewQuery, Response<BrochurePreviewDto>>
        {
            private readonly IBrochureDataService _dataService;

            public GetBrochurePreviewQueryHandler(IBrochureDataService dataService)
            {
                _dataService = dataService;
            }

            public async Task<Response<BrochurePreviewDto>> Handle(
                GetBrochurePreviewQuery request, CancellationToken cancellationToken)
            {
                var preview = await _dataService.GetPreviewAsync(request, cancellationToken);
                return new Response<BrochurePreviewDto>(preview);
            }
        }
    }
}
