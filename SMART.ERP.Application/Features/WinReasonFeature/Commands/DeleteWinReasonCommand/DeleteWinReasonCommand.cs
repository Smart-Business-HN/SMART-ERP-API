using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WinReasonFeature.Commands.DeleteWinReasonCommand
{
    public class DeleteWinReasonCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteWinReasonCommandHandler : IRequestHandler<DeleteWinReasonCommand, Response<string>>
    {
        private readonly IRepositoryAsync<WinReason> _repositoryAsync;

        public DeleteWinReasonCommandHandler(IRepositoryAsync<WinReason> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteWinReasonCommand request, CancellationToken cancellationToken)
        {
            var winReason = await _repositoryAsync.GetByIdAsync(request.Id);
            if (winReason == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(winReason);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{winReason.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
