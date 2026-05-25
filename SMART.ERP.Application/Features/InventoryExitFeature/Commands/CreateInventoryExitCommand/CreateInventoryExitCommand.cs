using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.InventoryExit;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryExitSpecification;
using SMART.ERP.Application.Specifications.PrefixSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.InventoryExitFeature.Commands.CreateInventoryExitCommand
{
    public class CreateInventoryExitCommand : IRequest<Response<InventoryExitDto>>
    {
        public DateTime ExitDate { get; set; }
        public InventoryExitReason ExitReason { get; set; }
        public string? CustomReason { get; set; }
        public int WarehouseId { get; set; }
        public int? PrefixId { get; set; }
        public string? Notes { get; set; }
        public string? BeneficiaryName { get; set; }
        public List<CreateInventoryExitItemDto> Items { get; set; } = [];

        public class CreateInventoryExitCommandHandler : IRequestHandler<CreateInventoryExitCommand, Response<InventoryExitDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<InventoryExit> _repositoryAsync;
            private readonly IReadRepositoryAsync<Warehouse> _warehouseRepository;
            private readonly IReadRepositoryAsync<Prefix> _prefixRepository;
            private readonly IReadRepositoryAsync<Product> _productRepository;

            public CreateInventoryExitCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<InventoryExit> repositoryAsync, IReadRepositoryAsync<Warehouse> warehouseRepository,
                IReadRepositoryAsync<Prefix> prefixRepository, IReadRepositoryAsync<Product> productRepository)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _warehouseRepository = warehouseRepository;
                _prefixRepository = prefixRepository;
                _productRepository = productRepository;
            }

            public async Task<Response<InventoryExitDto>> Handle(CreateInventoryExitCommand request, CancellationToken cancellationToken)
            {
                if (request.Items == null || request.Items.Count == 0)
                    throw new ApiException("La salida de inventario debe tener al menos un producto.");

                _ = await _warehouseRepository.GetByIdAsync(request.WarehouseId, cancellationToken)
                    ?? throw new ApiException($"No existe un almacén con el Id {request.WarehouseId}");

                var prefix = request.PrefixId.HasValue
                    ? await _prefixRepository.GetByIdAsync(request.PrefixId.Value, cancellationToken)
                    : await _prefixRepository.FirstOrDefaultAsync(new PrefixByFormatSpecification(InventoryPrefixes.Exit), cancellationToken);
                if (prefix == null)
                    throw new ApiException($"No existe el prefijo de Salida de Inventario ('{InventoryPrefixes.Exit}'). Configúralo en Prefijos.");

                foreach (var item in request.Items)
                {
                    _ = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new ApiException($"No existe un producto con el Id {item.ProductId}");
                    if (item.Quantity <= 0)
                        throw new ApiException("La cantidad de cada producto debe ser mayor a 0.");
                }

                var exit = new InventoryExit
                {
                    Status = InventoryExitStatus.Draft,
                    ExitDate = request.ExitDate == default ? DateTime.Now : request.ExitDate,
                    ExitReason = request.ExitReason,
                    CustomReason = request.CustomReason,
                    WarehouseId = request.WarehouseId,
                    PrefixId = prefix.Id,
                    Notes = request.Notes,
                    BeneficiaryName = request.BeneficiaryName,
                    CreationDate = DateTime.Now,
                    CreatedBy = _jwtService.GetSubjectToken(),
                    Items = request.Items.Select(i => new InventoryExitItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Notes = i.Notes
                    }).ToList()
                };

                var created = await _repositoryAsync.AddAsync(exit, cancellationToken);
                await _repositoryAsync.SaveChangesAsync(cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new GetInventoryExitByIdSpecification(created.Id), cancellationToken);
                var dto = _mapper.Map<InventoryExitDto>(full);
                return new Response<InventoryExitDto>(dto, "Salida de inventario creada correctamente.");
            }
        }
    }
}
