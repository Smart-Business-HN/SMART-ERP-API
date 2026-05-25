using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryEntry;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryEntrySpecification;
using SMART.ERP.Application.Specifications.PrefixSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryEntryFeature.Commands.CreateInventoryEntryCommand
{
    public class CreateInventoryEntryCommand : IRequest<Response<InventoryEntryDto>>
    {
        public InventoryEntryType EntryType { get; set; }
        public DateTime EntryDate { get; set; }
        public int WarehouseId { get; set; }
        public int? PrefixId { get; set; }
        public int? ProviderId { get; set; }
        public string? SupplierReference { get; set; }
        public string? Description { get; set; }
        public List<CreateInventoryEntryItemDto> Items { get; set; } = [];

        public class CreateInventoryEntryCommandHandler : IRequestHandler<CreateInventoryEntryCommand, Response<InventoryEntryDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<InventoryEntry> _repositoryAsync;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Provider> _providerRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;

            public CreateInventoryEntryCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<InventoryEntry> repositoryAsync, IReadRepositoryAsync<Warehouse> warehouseRepository,
                IReadRepositoryAsync<Prefix> prefixRepository, IReadRepositoryAsync<Provider> providerRepository,
                IReadRepositoryAsync<Product> productRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _warehouseRepository = warehouseRepository;
                _prefixRepository = prefixRepository;
                _providerRepository = providerRepository;
                _productRepository = productRepository;
            }

            public async Task<Response<InventoryEntryDto>> Handle(CreateInventoryEntryCommand request, CancellationToken cancellationToken)
            {
                if (request.Items == null || request.Items.Count == 0)
                    throw new ApiException("La entrada de inventario debe tener al menos un producto.");

                var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén con el Id {request.WarehouseId}");

                var prefix = request.PrefixId.HasValue
                    ? await _prefixRepository.GetByIdAsync(request.PrefixId.Value, cancellationToken)
                    : await _prefixRepository.FirstOrDefaultAsync(new PrefixByFormatSpecification(InventoryPrefixes.Entry), cancellationToken);
                if (prefix == null)
                    throw new ApiException($"No existe el prefijo de Entrada de Inventario ('{InventoryPrefixes.Entry}'). Configúralo en Prefijos.");

                if (request.EntryType == InventoryEntryType.Purchase && request.ProviderId.HasValue)
                {
                    _ = await _providerRepository.GetByIdAsync(request.ProviderId.Value, cancellationToken)
                        ?? throw new ApiException($"No existe un proveedor con el Id {request.ProviderId.Value}");
                }

                foreach (var item in request.Items)
                {
                    _ = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new ApiException($"No existe un producto con el Id {item.ProductId}");
                    if (item.Quantity <= 0 && request.EntryType != InventoryEntryType.Adjustment)
                        throw new ApiException("La cantidad de cada producto debe ser mayor a 0.");
                    if (item.Quantity < 0)
                        throw new ApiException("La cantidad de cada producto no puede ser negativa.");
                }

                var entry = new InventoryEntry
                {
                    EntryType = request.EntryType,
                    Status = InventoryEntryStatus.Draft,
                    EntryDate = request.EntryDate == default ? DateTime.Now : request.EntryDate,
                    WarehouseId = request.WarehouseId,
                    PrefixId = prefix.Id,
                    ProviderId = request.ProviderId,
                    SupplierReference = request.SupplierReference,
                    Description = request.Description,
                    CreationDate = DateTime.Now,
                    CreatedBy = _jwtService.GetSubjectToken(),
                    Items = request.Items.Select(i => new InventoryEntryItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitCost = i.UnitCost,
                        Total = i.Quantity * (i.UnitCost ?? 0m),
                        Notes = i.Notes
                    }).ToList()
                };

                var created = await _repositoryAsync.AddAsync(entry, cancellationToken);
                await _repositoryAsync.SaveChangesAsync(cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new GetInventoryEntryByIdSpecification(created.Id), cancellationToken);
                var dto = _mapper.Map<InventoryEntryDto>(full);
                return new Response<InventoryEntryDto>(dto, "Entrada de inventario creada correctamente.");
            }
        }
    }
}
