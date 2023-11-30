using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TaxFeature.Commands.DeleteTaxCommand
{
    public class DeleteTaxCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
    public class DeleteTaxCommandHandler: IRequestHandler<DeleteTaxCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Tax> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;
        public DeleteTaxCommandHandler(IRepositoryAsync<Tax> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteTaxCommand request, CancellationToken cancellationToken)
        {
            var tax = await _repositoryAsync.GetByIdAsync(request.Id);
            if(tax == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(tax);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_taxes", cancellationToken);
            return new Response<string>($"{tax.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
