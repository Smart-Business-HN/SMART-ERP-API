using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BrandSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand
{
    public class CreateBrandCommand : IRequest<Response<Brand>>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public string? Background { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Response<Brand>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Brand> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateBrandCommandHandler(IMapper mapper, IRepositoryAsync<Brand> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<Brand>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterBrandSpecification(request.Name, null));
            if (brand != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<Brand>(request);

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_brands",cancellationToken);
            return new Response<Brand>(data, message: $"{request.Name} creado exitosamente");
        }
    }
}
