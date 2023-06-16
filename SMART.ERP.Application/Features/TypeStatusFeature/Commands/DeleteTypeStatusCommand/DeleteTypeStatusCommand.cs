using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TypeStatusFeature.Commands.DeleteTypeStatusCommand
{
    public class DeleteTypeStatusCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteTypeStatusCommandHandler : IRequestHandler<DeleteTypeStatusCommand, Response<string>>
    {
        private readonly IRepositoryAsync<TypeStatus> _repositoryAsync;

        public DeleteTypeStatusCommandHandler(IRepositoryAsync<TypeStatus> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteTypeStatusCommand request, CancellationToken cancellationToken)
        {
            var checkTypeStatus = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkTypeStatus == null)
            {
                throw new KeyNotFoundException($"No se encontro el registro con Id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkTypeStatus);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado Correctamente");
            }
            catch (Exception)
            {

                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro");
            }
        }
    }
}
