using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BrandFeature.Commands.DeleteBrandCommand
{
    public class DeleteBrandCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Brand> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteBrandCommandHandler(IRepositoryAsync<Brand> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _repositoryAsync.GetByIdAsync(request.Id);
            if (brand == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(brand);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_brands", cancellationToken);
            return new Response<string>($"{brand.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
