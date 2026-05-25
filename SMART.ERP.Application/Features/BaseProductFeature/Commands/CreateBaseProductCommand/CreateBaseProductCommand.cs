using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.PriceListResolver;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.CreateBaseProductCommand
{
    public class CreateBaseProductCommand : IRequest<Response<ProductDto>>
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ProductType ProductType { get; set; } = ProductType.Tangible;
        public string? Description { get; set; }
        public string? Brochure { get; set; }
        public string? VirtualTour { get; set; }
        public string? UrlYoutube { get; set; }
        public bool IsFatherProduct { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public int MinStock { get; set; }
        public int CurrentStock { get; set; }
        public int BrandId { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public int SubCategoryId { get; set; }
        public int TaxId { get; set; }
        public int StatusId { get; set; }
        public int ProviderId { get; set; }
        public bool IsActive { get; set; }
        public bool ShowInEcommerce { get; set; }
        public string? EcommerceDescription { get; set; }
        public List<ProductPartInput>? Components { get; set; }
    }

    public class CreateBaseProductCommandHandler : IRequestHandler<CreateBaseProductCommand, Response<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryAsync<Subcategory> _subcategoryRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<Provider> _providerRepositoryAsync;
        private readonly IRepositoryAsync<Brand> _brandRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<UnitOfMeasurement> _measurementRepositoryAsync;
        private readonly IRepositoryAsync<PriceListItem> _priceListItemRepository;
        private readonly IRepositoryAsync<ProductPart> _productPartRepository;
        private readonly IPriceListService _priceListService;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateBaseProductCommandHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync,
            IJwtService jwtService, IRepositoryAsync<Subcategory> subcategoryRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync,
            IRepositoryAsync<Brand> brandRepositoryAsync,
            IRepositoryAsync<UnitOfMeasurement> measurementRepositoryAsync,
            IRepositoryAsync<PriceListItem> priceListItemRepository,
            IRepositoryAsync<ProductPart> productPartRepository,
            IPriceListService priceListService,
            IOutputCacheStore outputCacheStored
            )
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _subcategoryRepositoryAsync = subcategoryRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _brandRepositoryAsync = brandRepositoryAsync;
            _measurementRepositoryAsync = measurementRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _priceListItemRepository = priceListItemRepository;
            _productPartRepository = productPartRepository;
            _priceListService = priceListService;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<ProductDto>> Handle(CreateBaseProductCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new FilterProductSpecification(request.Name, null, Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-")));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var checkSubcategory = await _subcategoryRepositoryAsync.GetByIdAsync(request.SubCategoryId);
            if (checkSubcategory == null)
            {
                throw new KeyNotFoundException($"No se encontro la subcategoria con id {request.SubCategoryId}");
            }
            var checkTax = await _taxRepositoryAsync.GetByIdAsync(request.TaxId);
            if (checkTax == null)
            {
                throw new KeyNotFoundException($"No se encontro un impuesto con id {request.TaxId}");
            }
            var checkStatus = await _statusRepositoryAsync.GetByIdAsync(request.StatusId);
            if (checkStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el estado con id {request.StatusId}");
            }
            var checkProvider = await _providerRepositoryAsync.GetByIdAsync(request.ProviderId);
            if (checkProvider == null)
            {
                throw new KeyNotFoundException($"No se encontro el proveedor con id {request.ProviderId}");
            }
            var checkBrand = await _brandRepositoryAsync.GetByIdAsync(request.BrandId);
            if (checkBrand == null)
            {
                throw new KeyNotFoundException($"No se encontro la marca con id {request.BrandId}");
            }
            var checkMeasurement = await _measurementRepositoryAsync.GetByIdAsync(request.UnitOfMeasurementId);
            if (checkMeasurement == null)
            {
                throw new KeyNotFoundException($"No se encontro la unidad de medida con id {request.UnitOfMeasurementId}");
            }

            // Validar componentes si es Combo y cargar entidades para snapshot
            List<Product> componentEntities = new();
            if (request.ProductType == ProductType.Combo)
            {
                if (request.Components == null || request.Components.Count == 0)
                    throw new ApiException("Un combo requiere al menos un componente.");
                if (request.Components.Any(c => c.Quantity <= 0))
                    throw new ApiException("La cantidad de cada componente debe ser mayor a cero.");

                var ids = request.Components.Select(c => c.ProductId).Distinct().ToList();
                if (ids.Count != request.Components.Count)
                    throw new ApiException("No se permiten componentes duplicados.");

                componentEntities = (await _repositoryAsync.ListAsync(new ProductsByIdsSpecification(ids))).ToList();
                if (componentEntities.Count != ids.Count)
                    throw new ApiException("Uno o más componentes no existen.");
                foreach (var c in componentEntities)
                {
                    if (!c.IsActive)
                        throw new ApiException($"El componente {c.Name} no está activo.");
                    if (c.ProductType != ProductType.Tangible)
                        throw new ApiException($"El componente {c.Name} debe ser un producto tangible.");
                }
            }

            var slug = Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-");
            var newRecord = _mapper.Map<Product>(request);
            newRecord.Slug = slug;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;

            // Servicios y combos no manejan stock propio
            if (request.ProductType == ProductType.Service)
            {
                newRecord.CurrentStock = 0;
                newRecord.MinStock = 0;
                newRecord.IsFatherProduct = false;
            }
            else if (request.ProductType == ProductType.Combo)
            {
                newRecord.CurrentStock = 0;
                newRecord.MinStock = 0;
                newRecord.IsFatherProduct = true;
            }

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            // Insertar componentes si es combo
            if (request.ProductType == ProductType.Combo)
            {
                var createdBy = _jwtService.GetSubjectToken();
                var now = DateTime.Now;
                foreach (var input in request.Components!)
                {
                    var component = componentEntities.First(c => c.Id == input.ProductId);
                    await _productPartRepository.AddAsync(new ProductPart
                    {
                        FatherProductId = data.Id,
                        FatherProductCode = data.Code,
                        FatherProductName = data.Name,
                        ProductId = component.Id,
                        ProductCode = component.Code,
                        ProductName = component.Name,
                        Quantity = input.Quantity,
                        IsActive = true,
                        CreatedBy = createdBy,
                        CreationDate = now
                    }, cancellationToken);
                }
                await _productPartRepository.SaveChangesAsync(cancellationToken);
            }

            // Auto-poblar precio en lista default usando CostPrice × 1.30 × (1 + Tax)
            var defaultPriceListId = await _priceListService.GetDefaultPriceListIdAsync(cancellationToken);
            if (defaultPriceListId.HasValue && data.CostPrice > 0)
            {
                var taxRate = checkTax.Rate;
                var defaultPrice = Math.Ceiling((data.CostPrice * 1.30m) * (1m + (taxRate / 100m)));
                await _priceListItemRepository.AddAsync(new PriceListItem
                {
                    PriceListId = defaultPriceListId.Value,
                    ProductId = data.Id,
                    Price = defaultPrice,
                    CreationDate = DateTime.UtcNow,
                    CreatedBy = _jwtService.GetSubjectToken()
                }, cancellationToken);
                await _priceListItemRepository.SaveChangesAsync(cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_pricelists", cancellationToken);
            }

            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            var dto = _mapper.Map<ProductDto>(data);
            return new Response<ProductDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
