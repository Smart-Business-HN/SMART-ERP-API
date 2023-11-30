using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.DeleteBranchOfficeCommand
{
    public class DeleteBranchOfficeCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBranchOfficeCommandHandler : IRequestHandler<DeleteBranchOfficeCommand, Response<string>>
    {
        private readonly IRepositoryAsync<BranchOffices> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteBranchOfficeCommandHandler(IRepositoryAsync<BranchOffices> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteBranchOfficeCommand request, CancellationToken cancellationToken)
        {
            var branchOffice = await _repositoryAsync.GetByIdAsync(request.Id);
            if (branchOffice == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(branchOffice);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_branchOffices", cancellationToken);
            return new Response<string>($"{branchOffice.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
