using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlySaleDeclarationFeature.Commands.DeleteMonthlySaleDeclarationCommand
{
    public class DeleteMonthlySaleDeclarationCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteMonthlySaleDeclarationCommandHandler : IRequestHandler<DeleteMonthlySaleDeclarationCommand, Response<string>>
    {
        private readonly IRepositoryAsync<MonthlySaleDeclaration> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteMonthlySaleDeclarationCommandHandler(IRepositoryAsync<MonthlySaleDeclaration> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteMonthlySaleDeclarationCommand request, CancellationToken cancellationToken)
        {
            var declaration = await _repositoryAsync.GetByIdAsync(request.Id);
            if (declaration == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(declaration);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_monthlySaleDeclaration", cancellationToken);
            return new Response<string>($"{declaration.Period} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
