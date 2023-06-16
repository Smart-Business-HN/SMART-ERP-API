using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeOriginFeature.Commands.DeleteTypeOriginCommand
{
    public class DeleteTypeOriginCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteTypeOriginCommandHandler : IRequestHandler<DeleteTypeOriginCommand, Response<string>>
    {
        private readonly IRepositoryAsync<TypeOrigin> _repositoryAsync;

        public DeleteTypeOriginCommandHandler(IRepositoryAsync<TypeOrigin> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteTypeOriginCommand request, CancellationToken cancellationToken)
        {
            var typeOrigin = await _repositoryAsync.GetByIdAsync(request.Id);
            if (typeOrigin == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(typeOrigin);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{typeOrigin.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
