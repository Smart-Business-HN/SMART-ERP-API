using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.RemoveRegionDepartmentCommand
{
    public class RemoveRegionDepartmentCommand : IRequest<Response<string>>
    {
        public int DepartmentId { get; set; }
    }

    public class RemoveRegionDepartmentCommandHandler : IRequestHandler<RemoveRegionDepartmentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Department> _repositoryAsync;

        public RemoveRegionDepartmentCommandHandler(IRepositoryAsync<Department> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(RemoveRegionDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkDepartment = await _repositoryAsync.GetByIdAsync(request.DepartmentId);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el departamento con Id {request.DepartmentId}");
            }
            checkDepartment.RegionId = null;
            await _repositoryAsync.UpdateAsync(checkDepartment);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<string>("Asignacion eliminada con exito.");
        }
    }
}
