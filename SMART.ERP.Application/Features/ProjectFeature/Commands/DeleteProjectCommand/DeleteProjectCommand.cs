using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProjectFeature.Commands.DeleteProjectCommand
{
    public class DeleteProjectCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Project> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteProjectCommandHandler(IRepositoryAsync<Project> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<string>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _repositoryAsync.GetByIdAsync(request.Id);
            if (project == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun proyecto con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(project);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_project", cancellationToken);
            return new Response<string>("Proyecto eliminado correctamente", "Eliminado correctamente");
        }
    }
}
