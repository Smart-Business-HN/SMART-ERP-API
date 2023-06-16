using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProviderFeature.Commands.DeleteProviderCommand
{
    public class DeleteProviderCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteProviderCommandHandler : IRequestHandler<DeleteProviderCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Provider> _repositoryAsync;

        public DeleteProviderCommandHandler(IRepositoryAsync<Provider> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = await _repositoryAsync.GetByIdAsync(request.Id);
            if (provider == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(provider);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{provider.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
