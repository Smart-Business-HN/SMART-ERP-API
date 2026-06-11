using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.ProductSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.RestoreBaseProductCommand
{
    public class RestoreBaseProductCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class RestoreBaseProductCommandHandler : IRequestHandler<RestoreBaseProductCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public RestoreBaseProductCommandHandler(IRepositoryAsync<Product> repositoryAsync,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(RestoreBaseProductCommand request, CancellationToken cancellationToken)
        {
            // GetByIdAsync no ve productos eliminados (filtro global): se busca ignorando el filtro.
            var product = await _repositoryAsync.FirstOrDefaultAsync(new DeletedProductByIdSpecification(request.Id), cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun producto eliminado con el id {request.Id}");
            }

            product.IsDeleted = false;
            product.DeletedAt = null;
            product.DeletedBy = null;
            product.ModificatedBy = _jwtService.GetSubjectToken();
            product.ModificationDate = DateTime.Now;

            await _repositoryAsync.UpdateAsync(product, cancellationToken);
            await _repositoryAsync.SaveChangesAsync(cancellationToken);

            await ProductCacheEviction.EvictAsync(_outputCacheStored, cancellationToken);

            return new Response<string>($"{product.Name} restaurado correctamente", "Restaurado correctamente");
        }
    }
}
