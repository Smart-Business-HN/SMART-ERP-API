using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.DeleteOpportunityDocumentCommand
{
    public class DeleteOpportunityDocumentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteOpportunityDocumentCommandHandler : IRequestHandler<DeleteOpportunityDocumentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<OpportunityDocument> _repositoryAsync;

        public DeleteOpportunityDocumentCommandHandler(IRepositoryAsync<OpportunityDocument> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteOpportunityDocumentCommand request, CancellationToken cancellationToken)
        {
            var opportunityDocument = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityDocument == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(opportunityDocument);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Actividad eliminada correctamente", "Eliminado correctamente");
        }
    }
}
