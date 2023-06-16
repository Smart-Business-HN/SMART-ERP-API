using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeActivityFeature.Commands.DeleteTypeActivityCommand
{
    public class DeleteTypeActivityCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteTypeActivityCommandHandler : IRequestHandler<DeleteTypeActivityCommand, Response<string>>
    {
        private readonly IRepositoryAsync<TypeActivity> _repositoryAsync;

        public DeleteTypeActivityCommandHandler(IRepositoryAsync<TypeActivity> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteTypeActivityCommand request, CancellationToken cancellationToken)
        {
            var typeActivity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (typeActivity == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(typeActivity);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{typeActivity.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
