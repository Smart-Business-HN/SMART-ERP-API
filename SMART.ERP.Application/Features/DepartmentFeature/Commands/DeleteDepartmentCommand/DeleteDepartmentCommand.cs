using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DepartmentFeature.Commands.DeleteDepartmentCommand
{
    public class DeleteDepartmentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Department> _repositoryAsync;

        public DeleteDepartmentCommandHandler(IRepositoryAsync<Department> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkDepartment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkDepartment);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
