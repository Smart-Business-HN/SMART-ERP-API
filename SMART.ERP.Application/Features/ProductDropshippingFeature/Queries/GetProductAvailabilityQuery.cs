using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Services.VirtualStock;
using SMART.ERP.Application.Wrappers;

namespace SMART.ERP.Application.Features.ProductDropshippingFeature.Queries
{
    public class GetProductAvailabilityQuery : IRequest<Response<ProductAvailabilityDto>>
    {
        public int ProductId { get; set; }

        public class GetProductAvailabilityQueryHandler : IRequestHandler<GetProductAvailabilityQuery, Response<ProductAvailabilityDto>>
        {
            private readonly IVirtualStockService _virtualStockService;

            public GetProductAvailabilityQueryHandler(IVirtualStockService virtualStockService)
            {
                _virtualStockService = virtualStockService;
            }

            public async Task<Response<ProductAvailabilityDto>> Handle(GetProductAvailabilityQuery request, CancellationToken cancellationToken)
            {
                var availability = await _virtualStockService.GetProductAvailabilityAsync(request.ProductId);
                return new Response<ProductAvailabilityDto>(availability);
            }
        }
    }
}
