using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.CategorySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System.Text.RegularExpressions;

namespace SMART.ERP.Application.Features.CategoryFeature.Commands.CreateCategoryCommand
{
    public class CreateCategoryCommand : IRequest<Response<CategoryDto>>
    {
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public bool IsPartCategory { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Response<CategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Category> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateCategoryCommandHandler(
            IMapper mapper,
            IRepositoryAsync<Category> repositoryAsync,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.FirstOrDefaultAsync(new FilterCategorySpecification(request.Name, null));
            if (category != null)
            {
                throw new ApiException($"Ya existe una categoria con el nombre {request.Name}");
            }
            var slug = Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-");
            var newRecord = _mapper.Map<Category>(request);
            newRecord.Slug = slug;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_categories", cancellationToken);
            var dto = _mapper.Map<CategoryDto>(data);
            return new Response<CategoryDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
