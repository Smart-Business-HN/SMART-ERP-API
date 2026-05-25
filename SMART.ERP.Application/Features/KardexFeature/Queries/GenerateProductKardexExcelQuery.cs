using MediatR;
using SMART.ERP.Application.Services.KardexReportService;

namespace SMART.ERP.Application.Features.KardexFeature.Queries
{
    public class GenerateProductKardexExcelQuery : IRequest<byte[]>
    {
        public int ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeCancellations { get; set; }

        public class GenerateProductKardexExcelQueryHandler : IRequestHandler<GenerateProductKardexExcelQuery, byte[]>
        {
            private readonly IMediator _mediator;
            private readonly IKardexExcelService _excelService;

            public GenerateProductKardexExcelQueryHandler(IMediator mediator, IKardexExcelService excelService)
            {
                _mediator = mediator;
                _excelService = excelService;
            }

            public async Task<byte[]> Handle(GenerateProductKardexExcelQuery request, CancellationToken cancellationToken)
            {
                var response = await _mediator.Send(new GetProductKardexQuery
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    IncludeCancellations = request.IncludeCancellations
                }, cancellationToken);

                return await _excelService.GenerateExcelAsync(response.Data!);
            }
        }
    }
}
