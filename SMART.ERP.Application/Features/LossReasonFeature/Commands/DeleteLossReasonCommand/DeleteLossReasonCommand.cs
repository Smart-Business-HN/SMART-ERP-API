using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LossReasonFeature.Commands.DeleteLossReasonCommand
{
    public class DeleteLossReasonCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteLossReasonCommandHandler : IRequestHandler<DeleteLossReasonCommand, Response<string>>
    {
        private readonly IRepositoryAsync<LossReason> _repositoryAsync;

        public DeleteLossReasonCommandHandler(IRepositoryAsync<LossReason> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteLossReasonCommand request, CancellationToken cancellationToken)
        {
            var lossReason = await _repositoryAsync.GetByIdAsync(request.Id);
            if (lossReason == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(lossReason);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{lossReason.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }

}
