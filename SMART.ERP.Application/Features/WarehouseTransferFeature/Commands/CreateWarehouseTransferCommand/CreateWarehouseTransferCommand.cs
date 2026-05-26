using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.WarehouseTransfer;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.PrefixSpecification;
using SMART.ERP.Application.Specifications.WarehouseTransferSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.WarehouseTransferFeature.Commands.CreateWarehouseTransferCommand
{
    public class CreateWarehouseTransferCommand : IRequest<Response<WarehouseTransferDto>>
    {
        public DateTime TransferDate { get; set; }
        public int OriginWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public int? PrefixId { get; set; }
        public string? Notes { get; set; }
        public List<CreateWarehouseTransferItemDto> Items { get; set; } = [];

        public class CreateWarehouseTransferCommandHandler : IRequestHandler<CreateWarehouseTransferCommand, Response<WarehouseTransferDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<WarehouseTransfer> _repositoryAsync;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;

            public CreateWarehouseTransferCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<WarehouseTransfer> repositoryAsync, IReadRepositoryAsync<Warehouse> warehouseRepository,
                IReadRepositoryAsync<Prefix> prefixRepository, IReadRepositoryAsync<Product> productRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _warehouseRepository = warehouseRepository;
                _prefixRepository = prefixRepository;
                _productRepository = productRepository;
            }

            public async Task<Response<WarehouseTransferDto>> Handle(CreateWarehouseTransferCommand request, CancellationToken cancellationToken)
            {
                if (request.Items == null || request.Items.Count == 0)
                    throw new ApiException("La transferencia debe tener al menos un producto.");

                if (request.OriginWarehouseId == request.DestinationWarehouseId)
                    throw new ApiException("El almacén de origen y destino no pueden ser el mismo.");

                var originWarehouse = await _warehouseRepository.GetByIdAsync(request.OriginWarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén de origen con el Id {request.OriginWarehouseId}");
                var destinationWarehouse = await _warehouseRepository.GetByIdAsync(request.DestinationWarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén de destino con el Id {request.DestinationWarehouseId}");

                if (originWarehouse.IsVirtual || destinationWarehouse.IsVirtual)
                    throw new ApiException("No se permite transferir inventario hacia o desde almacenes virtuales (consignados).");
                var prefix = request.PrefixId.HasValue
                    ? await _prefixRepository.GetByIdAsync(request.PrefixId.Value, cancellationToken)
                    : await _prefixRepository.FirstOrDefaultAsync(new PrefixByFormatSpecification(InventoryPrefixes.Transfer), cancellationToken);
                if (prefix == null)
                    throw new ApiException($"No existe el prefijo de Transferencia ('{InventoryPrefixes.Transfer}'). Configúralo en Prefijos.");

                foreach (var item in request.Items)
                {
                    _ = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new ApiException($"No existe un producto con el Id {item.ProductId}");
                    if (item.Quantity <= 0)
                        throw new ApiException("La cantidad de cada producto debe ser mayor a 0.");
                }

                var transfer = new WarehouseTransfer
                {
                    Status = WarehouseTransferStatus.Draft,
                    TransferDate = request.TransferDate == default ? DateTime.Now : request.TransferDate,
                    OriginWarehouseId = request.OriginWarehouseId,
                    DestinationWarehouseId = request.DestinationWarehouseId,
                    PrefixId = prefix.Id,
                    Notes = request.Notes,
                    CreationDate = DateTime.Now,
                    CreatedBy = _jwtService.GetSubjectToken(),
                    Items = request.Items.Select(i => new WarehouseTransferItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                var created = await _repositoryAsync.AddAsync(transfer, cancellationToken);
                await _repositoryAsync.SaveChangesAsync(cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new GetWarehouseTransferByIdSpecification(created.Id), cancellationToken);
                var dto = _mapper.Map<WarehouseTransferDto>(full);
                return new Response<WarehouseTransferDto>(dto, "Transferencia creada correctamente.");
            }
        }
    }
}
