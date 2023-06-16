using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AdvisorDepartmentFeature.Commands.DeleteAdvisorDepartmentCommand
{
    public class DeleteAdvisorDepartmentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteAdvisorDepartmentCommandHandler : IRequestHandler<DeleteAdvisorDepartmentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<AdvisorDepartment> _repositoryAsync;

        public DeleteAdvisorDepartmentCommandHandler(IRepositoryAsync<AdvisorDepartment> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteAdvisorDepartmentCommand request, CancellationToken cancellationToken)
        {
            var checkAssignation = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAssignation == null)
            {
                throw new KeyNotFoundException($"No se encontro la asignacion con id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(checkAssignation);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>("Asignacion eliminada con exito.");
        }
    }
}
