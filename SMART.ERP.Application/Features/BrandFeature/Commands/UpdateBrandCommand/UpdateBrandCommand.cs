using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.BrandSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BrandFeature.Commands.UpdateBrandCommand
{
    public class UpdateBrandCommand : IRequest<Response<Brand>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public string? Background { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Response<Brand>>
    {
        private readonly IRepositoryAsync<Brand> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateBrandCommandHandler(IRepositoryAsync<Brand> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<Brand>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repositoryAsync.GetByIdAsync(request.Id);
            if (brand == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterBrandSpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            else
            {
                brand.Name = request.Name;
                brand.Description = request.Description;
                brand.Logo = request.Logo;
                brand.Background = request.Background;
                brand.IsActive = request.IsActive;
                await _repositoryAsync.UpdateAsync(brand);
                await _repositoryAsync.SaveChangesAsync();
                await _outputCacheStored.EvictByTagAsync("cache_brands", cancellationToken);
                return new Response<Brand>(brand, message: $"{brand.Name} actualizado correctamente");
            }
        }
    }
}
