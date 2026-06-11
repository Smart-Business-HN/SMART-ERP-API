using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BaseProductFeature.Commands.DeleteBaseProductCommand
{
    public class DeleteBaseProductCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBaseProductCommandHandler : IRequestHandler<DeleteBaseProductCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Product> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteBaseProductCommandHandler(IRepositoryAsync<Product> repositoryAsync,
            IJwtService jwtService,
            IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteBaseProductCommand request, CancellationToken cancellationToken)
        {
            // GetByIdAsync respeta el filtro global, por lo que un producto ya eliminado
            // devuelve null aqui (eliminar dos veces es un no-op/404).
            var product = await _repositoryAsync.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }

            // Soft delete: se conservan el producto y sus hijos (datasheets/features/images)
            // para historial y para poder restaurarlo.
            product.IsDeleted = true;
            product.DeletedAt = DateTime.Now;
            product.DeletedBy = _jwtService.GetSubjectToken();

            await _repositoryAsync.UpdateAsync(product);
            await _repositoryAsync.SaveChangesAsync();

            await ProductCacheEviction.EvictAsync(_outputCacheStored, cancellationToken);

            return new Response<string>($"{product.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
