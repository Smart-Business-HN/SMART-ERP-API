using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DepartmentSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RegionFeature.Commands.DeleteRegionCommand
{
    public class DeleteRegionCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteRegionCommandHandler : IRequestHandler<DeleteRegionCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Region> _repositoryAsync;
        private readonly IRepositoryAsync<Department> _departmentRepositoryAsync;

        public DeleteRegionCommandHandler(IRepositoryAsync<Region> repositoryAsync, IRepositoryAsync<Department> departmentRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _departmentRepositoryAsync = departmentRepositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
        {
            var checkRegion = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkRegion == null)
            {
                throw new KeyNotFoundException($"No se encontro la region con Id {request.Id}");
            }
            var departments = await _departmentRepositoryAsync.ListAsync(new FilterDepartmentByRegionSpecification(checkRegion.Id));
            if (departments.Count > 0)
            {
                departments.ForEach(x => x.RegionId = null);
                await _departmentRepositoryAsync.UpdateRangeAsync(departments);
                await _departmentRepositoryAsync.SaveChangesAsync();
            }
            try
            {
                await _repositoryAsync.DeleteAsync(checkRegion);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado exitosamente");
            }
            catch (Exception ex)
            {

                throw new ApiException("Ha ocurrido un error la tratar de eliminar esta region, revisa sus referencias.");
            }

        }
    }
}
