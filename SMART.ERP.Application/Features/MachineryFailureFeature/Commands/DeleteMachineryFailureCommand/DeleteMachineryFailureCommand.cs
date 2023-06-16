using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Commands.DeleteMachineryFailureCommand
{
    public class DeleteMachineryFailureCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteMachineryFailureCommandHandler : IRequestHandler<DeleteMachineryFailureCommand, Response<string>>
    {
        private readonly IRepositoryAsync<MachineryFailure> _repositoryAsync;

        public DeleteMachineryFailureCommandHandler(IRepositoryAsync<MachineryFailure> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteMachineryFailureCommand request, CancellationToken cancellationToken)
        {
            var machineryFailure = await _repositoryAsync.GetByIdAsync(request.Id);
            if (machineryFailure == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(machineryFailure);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{machineryFailure.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
