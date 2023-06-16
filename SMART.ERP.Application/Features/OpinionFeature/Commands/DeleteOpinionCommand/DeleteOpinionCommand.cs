using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpinionFeature.Commands.DeleteOpinionCommand
{
    public class DeleteOpinionCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteOpinionCommandHandler : IRequestHandler<DeleteOpinionCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Opinion> _repositoryAsync;

        public DeleteOpinionCommandHandler(IRepositoryAsync<Opinion> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteOpinionCommand request, CancellationToken cancellationToken)
        {
            var opinion = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opinion == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(opinion);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{opinion.Title} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
