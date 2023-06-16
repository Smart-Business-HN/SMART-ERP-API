using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityStepFeature.Command.DeleteOpportunityStepCommand
{
    public class DeleteOpportunityStepCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteOpportunityStepCommandHandler : IRequestHandler<DeleteOpportunityStepCommand, Response<string>>
    {
        private readonly IRepositoryAsync<OpportunityStep> _repositoryAsync;

        public DeleteOpportunityStepCommandHandler(IRepositoryAsync<OpportunityStep> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteOpportunityStepCommand request, CancellationToken cancellationToken)
        {
            var opportunityStep = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityStep != null)
            {
                await _repositoryAsync.DeleteAsync(opportunityStep);
                await _repositoryAsync.SaveChangesAsync();
                return new Response<string>($"Registro {opportunityStep.Name} eliminado correctamente", "Eliminado correctamente");
            }
            else
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
        }
    }
}
