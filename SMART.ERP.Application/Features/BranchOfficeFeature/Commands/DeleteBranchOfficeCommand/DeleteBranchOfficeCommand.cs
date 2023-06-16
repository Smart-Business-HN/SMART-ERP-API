using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.DeleteBranchOfficeCommand
{
    public class DeleteBranchOfficeCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBranchOfficeCommandHandler : IRequestHandler<DeleteBranchOfficeCommand, Response<string>>
    {
        private readonly IRepositoryAsync<BranchOffices> _repositoryAsync;

        public DeleteBranchOfficeCommandHandler(IRepositoryAsync<BranchOffices> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteBranchOfficeCommand request, CancellationToken cancellationToken)
        {
            var branchOffice = await _repositoryAsync.GetByIdAsync(request.Id);
            if (branchOffice == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(branchOffice);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{branchOffice.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
