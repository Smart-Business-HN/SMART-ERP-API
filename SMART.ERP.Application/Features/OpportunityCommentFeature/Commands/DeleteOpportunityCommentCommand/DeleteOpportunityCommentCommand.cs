using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.DeleteOpportunityCommentCommand
{
    public class DeleteOpportunityCommentCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteOpportunityCommentCommandHandler : IRequestHandler<DeleteOpportunityCommentCommand, Response<string>>
    {
        private readonly IRepositoryAsync<OpportunityComment> _repositoryAsync;

        public DeleteOpportunityCommentCommandHandler(IRepositoryAsync<OpportunityComment> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteOpportunityCommentCommand request, CancellationToken cancellationToken)
        {
            var opportunityComment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityComment == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(opportunityComment);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Actividad eliminada correctamente", "Eliminado correctamente");
        }
    }
}
