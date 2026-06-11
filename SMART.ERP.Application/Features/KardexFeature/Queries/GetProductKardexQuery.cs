using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Kardex;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InventoryMovementSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.KardexFeature.Queries
{
    public class GetProductKardexQuery : IRequest<Response<KardexReportDto>>
    {
        public int ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeCancellations { get; set; }

        public class GetProductKardexQueryHandler : IRequestHandler<GetProductKardexQuery, Response<KardexReportDto>>
        {
            private readonly IMapper _mapper;
            private readonly IReadRepositoryAsync<InventoryMovement> _movementRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;

            public GetProductKardexQueryHandler(IMapper mapper, IReadRepositoryAsync<InventoryMovement> movementRepository,
                IReadRepositoryAsync<Product> productRepository, IReadRepositoryAsync<Warehouse> warehouseRepository)
            {
                _mapper = mapper;
                _movementRepository = movementRepository;
                _productRepository = productRepository;
                _warehouseRepository = warehouseRepository;
            }

            public async Task<Response<KardexReportDto>> Handle(GetProductKardexQuery request, CancellationToken cancellationToken)
            {
                // Ignora el filtro de soft delete: un producto eliminado puede tener movimientos historicos.
                var product = await _productRepository.FirstOrDefaultAsync(
                        new ProductByIdIgnoreFiltersSpecification(request.ProductId), cancellationToken)
                    ?? throw new ApiException($"No existe un producto con el Id {request.ProductId}");

                var report = new KardexReportDto
                {
                    ProductId = product.Id,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    WarehouseId = request.WarehouseId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                };

                if (request.WarehouseId.HasValue)
                {
                    var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId.Value, cancellationToken);
                    report.WarehouseName = warehouse?.Name;
                }

                if (request.StartDate.HasValue)
                {
                    var opening = await _movementRepository.FirstOrDefaultAsync(
                        new GetOpeningMovementSpecification(request.ProductId, request.WarehouseId, request.StartDate.Value), cancellationToken);
                    if (opening != null)
                    {
                        report.OpeningQuantity = opening.RunningQuantity;
                        report.OpeningAverageCost = opening.RunningAverageCost;
                        report.OpeningTotalValue = opening.RunningTotalValue;
                    }
                }

                var movements = await _movementRepository.ListAsync(
                    new GetMovementsByProductSpecification(request.ProductId, request.WarehouseId, request.StartDate, request.EndDate, request.IncludeCancellations),
                    cancellationToken);

                report.Movements = _mapper.Map<List<KardexMovementDto>>(movements);
                report.Movements.ForEach(m => m.MovementTypeName = KardexMovementTypeNames.GetName(m.MovementType));

                report.TotalIn = movements.Sum(x => x.QuantityIn);
                report.TotalOut = movements.Sum(x => x.QuantityOut);

                var lastMovement = movements.LastOrDefault();
                if (lastMovement != null)
                {
                    report.ClosingQuantity = lastMovement.RunningQuantity;
                    report.ClosingAverageCost = lastMovement.RunningAverageCost;
                    report.ClosingTotalValue = lastMovement.RunningTotalValue;
                }
                else
                {
                    report.ClosingQuantity = report.OpeningQuantity;
                    report.ClosingAverageCost = report.OpeningAverageCost;
                    report.ClosingTotalValue = report.OpeningTotalValue;
                }

                return new Response<KardexReportDto>(report);
            }
        }
    }
}
