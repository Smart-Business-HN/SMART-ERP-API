using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.StatusFeature.Commands.DeleteStatusCommand
{
    public class DeleteStatusCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteStatusCommandHandler : IRequestHandler<DeleteStatusCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Status> _repositoryAsync;

        public DeleteStatusCommandHandler(IRepositoryAsync<Status> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
        {
            var checkStatus = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el estado con id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkStatus);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error la tratar de eliminar este registro, verifica que no se utilice en otro registro.");
            }
        }
    }
}
