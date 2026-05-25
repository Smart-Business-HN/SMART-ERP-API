using AutoMapper;
using Ardalis.Specification;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.InventoryMovementSpecification;
using SMART.ERP.Application.Specifications.ProductPartSpecification;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.UpdateBaseProductCommand
{
    public class UpdateBaseProductCommand : IRequest<Response<ProductDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ProductType ProductType { get; set; } = ProductType.Tangible;
        public string? Description { get; set; }
        public string? Brochure { get; set; }
        public string? VirtualTour { get; set; }
        public string? UrlYoutube { get; set; }
        public bool IsFatherProduct { get; set; }
        public int MinStock { get; set; }
        public int BrandId { get; set; }
        public int TaxId { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public int SubCategoryId { get; set; }
        public int StatusId { get; set; }
        public int ProviderId { get; set; }
        public bool IsActive { get; set; }
        public bool ShowInEcommerce { get; set; }
        public decimal CostPrice { get; set; }
        public decimal RecomendedSalePrice { get; set; }
        public int CurrentStock { get; set; }
        public string? EcommerceDescription { get; set; }
        public List<ProductPartInput>? Components { get; set; }
    }

    public class UpdateBaseProductCommandHandler : IRequestHandler<UpdateBaseProductCommand, Response<ProductDto>>
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
        private readonly IRepositoryAsync<ProductPart> _productPartRepository;
        private readonly IReadRepositoryAsync<InventoryMovement> _inventoryMovementRepository;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateBaseProductCommandHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync, IRepositoryAsync<Tax> taxRepositoryAsync,
            IJwtService jwtService, IRepositoryAsync<Subcategory> subcategoryRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync,
            IRepositoryAsync<Brand> brandRepositoryAsync,
            IRepositoryAsync<UnitOfMeasurement> measurementRepositoryAsync,
            IRepositoryAsync<ProductPart> productPartRepository,
            IReadRepositoryAsync<InventoryMovement> inventoryMovementRepository,
            IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _subcategoryRepositoryAsync = subcategoryRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _brandRepositoryAsync = brandRepositoryAsync;
            _measurementRepositoryAsync = measurementRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _productPartRepository = productPartRepository;
            _inventoryMovementRepository = inventoryMovementRepository;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<ProductDto>> Handle(UpdateBaseProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repositoryAsync.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
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
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterProductSpecification(request.Name, request.Id, null));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            // Prohibir cambiar ProductType si el producto ya tiene movimientos de Kardex
            if (product.ProductType != request.ProductType)
            {
                var hasMovements = await _inventoryMovementRepository
                    .AnyAsync(new HasMovementsForProductSpecification(product.Id), cancellationToken);
                if (hasMovements)
                {
                    throw new ApiException("No se puede cambiar el tipo del producto porque ya tiene movimientos de inventario.");
                }
            }

            // Validar componentes si es Combo
            List<Product> componentEntities = new();
            if (request.ProductType == ProductType.Combo)
            {
                if (request.Components == null || request.Components.Count == 0)
                    throw new ApiException("Un combo requiere al menos un componente.");
                if (request.Components.Any(c => c.Quantity <= 0))
                    throw new ApiException("La cantidad de cada componente debe ser mayor a cero.");
                if (request.Components.Any(c => c.ProductId == product.Id))
                    throw new ApiException("Un combo no puede contenerse a sí mismo.");

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

            product.Slug = Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-");
            product.Name = request.Name;
            product.ProductType = request.ProductType;
            product.Description = request.Description;
            product.BrandId = request.BrandId;
            product.Brochure = request.Brochure;
            product.VirtualTour = request.VirtualTour;
            product.TaxId = request.TaxId;
            product.UrlYoutube = request.UrlYoutube;
            product.ProviderId = request.ProviderId;
            product.StatusId = request.StatusId;
            product.ShowInEcommerce = request.ShowInEcommerce;
            product.UnitOfMeasurementId = request.UnitOfMeasurementId;
            product.SubCategoryId = request.SubCategoryId;
            product.IsActive = request.IsActive;
            product.ModificatedBy = _jwtService.GetSubjectToken();
            product.ModificationDate = DateTime.Now;
            product.CostPrice = request.CostPrice;
            product.RecomendedSalePrice = request.RecomendedSalePrice;
            product.EcommerceDescription = request.EcommerceDescription;

            if (request.ProductType == ProductType.Service)
            {
                product.CurrentStock = 0;
                product.MinStock = 0;
                product.IsFatherProduct = false;
            }
            else if (request.ProductType == ProductType.Combo)
            {
                product.CurrentStock = 0;
                product.MinStock = 0;
                product.IsFatherProduct = true;
            }
            else
            {
                product.MinStock = request.MinStock;
                product.CurrentStock = request.CurrentStock;
                product.IsFatherProduct = request.IsFatherProduct;
            }

            await _repositoryAsync.UpdateAsync(product);
            await _repositoryAsync.SaveChangesAsync();

            await ReconcileComponentsAsync(product, request, componentEntities, cancellationToken);

            await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
            await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
            var dto = _mapper.Map<ProductDto>(product);
            return new Response<ProductDto>(dto, message: $"{product.Name} actualizado correctamente");
        }

        private async Task ReconcileComponentsAsync(Product father, UpdateBaseProductCommand request, List<Product> componentEntities, CancellationToken cancellationToken)
        {
            var existing = (await _productPartRepository
                .ListAsync(new ProductPartsByFatherSpecification(father.Id), cancellationToken)).ToList();

            if (request.ProductType != ProductType.Combo)
            {
                if (existing.Count > 0)
                {
                    await _productPartRepository.DeleteRangeAsync(existing, cancellationToken);
                    await _productPartRepository.SaveChangesAsync(cancellationToken);
                }
                return;
            }

            var createdBy = _jwtService.GetSubjectToken();
            var now = DateTime.Now;
            var incomingIds = request.Components!.Select(c => c.ProductId).ToHashSet();

            var toRemove = existing.Where(e => !incomingIds.Contains(e.ProductId)).ToList();
            if (toRemove.Count > 0)
                await _productPartRepository.DeleteRangeAsync(toRemove, cancellationToken);

            var toAdd = new List<ProductPart>();
            var toUpdate = new List<ProductPart>();
            foreach (var input in request.Components!)
            {
                var component = componentEntities.First(c => c.Id == input.ProductId);
                var current = existing.FirstOrDefault(e => e.ProductId == input.ProductId);
                if (current == null)
                {
                    toAdd.Add(new ProductPart
                    {
                        FatherProductId = father.Id,
                        FatherProductCode = father.Code,
                        FatherProductName = father.Name,
                        ProductId = component.Id,
                        ProductCode = component.Code,
                        ProductName = component.Name,
                        Quantity = input.Quantity,
                        IsActive = true,
                        CreatedBy = createdBy,
                        CreationDate = now
                    });
                }
                else
                {
                    current.Quantity = input.Quantity;
                    current.IsActive = true;
                    current.ProductCode = component.Code;
                    current.ProductName = component.Name;
                    current.FatherProductCode = father.Code;
                    current.FatherProductName = father.Name;
                    current.ModificatedBy = createdBy;
                    current.ModificationDate = now;
                    toUpdate.Add(current);
                }
            }
            if (toAdd.Count > 0)
                await _productPartRepository.AddRangeAsync(toAdd, cancellationToken);
            if (toUpdate.Count > 0)
                await _productPartRepository.UpdateRangeAsync(toUpdate, cancellationToken);
            await _productPartRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
