using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.SubcategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.SubcategoryFeature.Commands.UpdateSubcategoryCommand
{
    public class UpdateSubcategoryCommand : IRequest<Response<SubcategoryDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateSubCategoryCommandHandler : IRequestHandler<UpdateSubcategoryCommand, Response<SubcategoryDto>>
    {
        private readonly IRepositoryAsync<Subcategory> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateSubCategoryCommandHandler(IMapper mapper, IRepositoryAsync<Subcategory> repositoryAsync,
            IJwtService jwtService, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<SubcategoryDto>> Handle(UpdateSubcategoryCommand request, CancellationToken cancellationToken)
        {
            var subcategory = await _repositoryAsync.GetByIdAsync(request.Id);
            if (subcategory == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterSubcategorySpecification(request.Name, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe una categoria con el nombre {request.Name}");
            }
            subcategory.Slug = Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-");
            subcategory.Name = request.Name;
            subcategory.CategoryId = request.CategoryId;
            subcategory.IsActive = request.IsActive;
            subcategory.ModificationDate = DateTime.Now;
            subcategory.ModificatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.UpdateAsync(subcategory);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_subCategories", cancellationToken);
            var dto = _mapper.Map<SubcategoryDto>(subcategory);
            return new Response<SubcategoryDto>(dto, message: $"{subcategory.Name} actualizado correctamente");
        }
    }
}
