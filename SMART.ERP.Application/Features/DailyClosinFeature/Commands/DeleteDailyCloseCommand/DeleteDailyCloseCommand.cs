using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DailyClosinFeature.Commands.DeleteDailyCloseCommand
{
    public class DeleteDailyCloseCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
    public class DeleteDailyCloseCommandHandler : IRequestHandler<DeleteDailyCloseCommand, Response<string>>
    {
        private readonly IRepositoryAsync<DailyClose> _repositoryAsync;
        public DeleteDailyCloseCommandHandler(IRepositoryAsync<DailyClose> repoAsync) { _repositoryAsync = repoAsync; }
        public async Task<Response<string>> Handle(DeleteDailyCloseCommand request, CancellationToken cancellationToken)
        {
            var dailyClose = await _repositoryAsync.GetByIdAsync(request.Id);
            if (dailyClose == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(dailyClose);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"Cierre del {dailyClose.Date} eliminado correctamente", "Eliminado Correctamente");
        }
    }
}
