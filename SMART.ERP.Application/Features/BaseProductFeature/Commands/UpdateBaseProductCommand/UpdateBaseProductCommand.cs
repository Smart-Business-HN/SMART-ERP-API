using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.UpdateBaseProductCommand
{
    public class UpdateBaseProductCommand : IRequest<Response<ProductDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
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
        public int CurrentStock { get; set;}
        public string? EcommerceDescription { get; set; }
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
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateBaseProductCommandHandler(IMapper mapper, IRepositoryAsync<Product> repositoryAsync,IRepositoryAsync<Tax> taxRepositoryAsync,
            IJwtService jwtService, IRepositoryAsync<Subcategory> subcategoryRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync, IRepositoryAsync<Provider> providerRepositoryAsync,
            IRepositoryAsync<Brand> brandRepositoryAsync,
            IRepositoryAsync<UnitOfMeasurement> measurementRepositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _subcategoryRepositoryAsync = subcategoryRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _providerRepositoryAsync = providerRepositoryAsync;
            _brandRepositoryAsync = brandRepositoryAsync;
            _measurementRepositoryAsync = measurementRepositoryAsync;
            _taxRepositoryAsync= taxRepositoryAsync;
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
            
                product.Slug = Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-");
                product.Name = request.Name;
                product.Description = request.Description;
                product.BrandId = request.BrandId;
                product.Brochure = request.Brochure;
                product.VirtualTour = request.VirtualTour;
                product.TaxId = request.TaxId;
                product.UrlYoutube = request.UrlYoutube;
                product.ProviderId = request.ProviderId;
                product.MinStock = request.MinStock;
                product.StatusId = request.StatusId;
                product.ShowInEcommerce = request.ShowInEcommerce;
                product.UnitOfMeasurementId = request.UnitOfMeasurementId;
                product.SubCategoryId = request.SubCategoryId;
                product.IsActive = request.IsActive;
                product.IsFatherProduct = request.IsFatherProduct;
                product.ModificatedBy = _jwtService.GetSubjectToken();
                product.ModificationDate = DateTime.Now;
                product.CostPrice = request.CostPrice;
                product.CurrentStock = request.CurrentStock;
                product.RecomendedSalePrice = request.RecomendedSalePrice;
                product.EcommerceDescription = request.EcommerceDescription;
            await _repositoryAsync.UpdateAsync(product);
                await _repositoryAsync.SaveChangesAsync();
                await _outputCacheStored.EvictByTagAsync("cache_products", cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_productsEcommerce", cancellationToken);
                var dto = _mapper.Map<ProductDto>(product);
                return new Response<ProductDto>(dto, message: $"{product.Name} actualizado correctamente");
            
        }
    }
}
