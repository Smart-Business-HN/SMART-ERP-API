using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.DeleteOpportunityActivityCommand
{
    public class DeleteOpportunityActivityCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteOpportunityActivityCommandHandler : IRequestHandler<DeleteOpportunityActivityCommand, Response<string>>
    {
        private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;

        public DeleteOpportunityActivityCommandHandler(IRepositoryAsync<OpportunityActivity> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteOpportunityActivityCommand request, CancellationToken cancellationToken)
        {
            var opportunityActivity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityActivity == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(opportunityActivity);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Actividad eliminada correctamente", "Eliminado correctamente");
        }
    }
}
