using MediatR;
using SMART.ERP.Application.Services.KardexReportService;

namespace SMART.ERP.Application.Features.KardexFeature.Queries
{
    public class GenerateProductKardexPdfQuery : IRequest<byte[]>
    {
        public int ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeCancellations { get; set; }

        public class GenerateProductKardexPdfQueryHandler : IRequestHandler<GenerateProductKardexPdfQuery, byte[]>
        {
            private readonly IMediator _mediator;
            private readonly IKardexPdfService _pdfService;

            public GenerateProductKardexPdfQueryHandler(IMediator mediator, IKardexPdfService pdfService)
            {
                _mediator = mediator;
                _pdfService = pdfService;
            }

            public async Task<byte[]> Handle(GenerateProductKardexPdfQuery request, CancellationToken cancellationToken)
            {
                var response = await _mediator.Send(new GetProductKardexQuery
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    IncludeCancellations = request.IncludeCancellations
                }, cancellationToken);

                return await _pdfService.GeneratePdfAsync(response.Data!);
            }
        }
    }
}
