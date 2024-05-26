using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Commands.DeleteMonthlyPurchaseDeclarationCommand
{
    public class DeleteMonthlyPurchaseDeclarationCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteMonthlyPurchaseDeclarationCommandHandler : IRequestHandler<DeleteMonthlyPurchaseDeclarationCommand, Response<string>>
    {
        private readonly IRepositoryAsync<MonthlyPurchaseDeclaration> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteMonthlyPurchaseDeclarationCommandHandler(IRepositoryAsync<MonthlyPurchaseDeclaration> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteMonthlyPurchaseDeclarationCommand request, CancellationToken cancellationToken)
        {
            var provider = await _repositoryAsync.GetByIdAsync(request.Id);
            if (provider == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(provider);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_monthlyPurchaseDeclaration", cancellationToken);
            return new Response<string>($"{provider.Period} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
