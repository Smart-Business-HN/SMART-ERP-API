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

namespace SMART.ERP.Application.Features.SubcategoryFeature.Commands.CreateSubcategoryCommand
{
    public class CreateSubcategoryCommand : IRequest<Response<SubcategoryDto>>
    {
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubcategoryCommandHandler : IRequestHandler<CreateSubcategoryCommand, Response<SubcategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Subcategory> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateSubcategoryCommandHandler(IMapper mapper, IRepositoryAsync<Subcategory> repositoryAsync,
            IJwtService jwtService, IOutputCacheStore outputCacheStored)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<SubcategoryDto>> Handle(CreateSubcategoryCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterSubcategorySpecification(request.Name, null));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var slug = Regex.Replace(Regex.Replace(request.Name, @"[^a-zA-Z0-9\s]", "").Trim().ToLower(), @"\s+", "-");
            var newRecord = _mapper.Map<Subcategory>(request);
            newRecord.Slug = slug;
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_subCategories", cancellationToken);
            var dto = _mapper.Map<SubcategoryDto>(data);

            return new Response<SubcategoryDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
