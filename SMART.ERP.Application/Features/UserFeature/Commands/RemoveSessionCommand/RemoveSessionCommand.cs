using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UserFeature.Commands.RemoveSessionCommand
{
    public class RemoveSessionCommand : IRequest<Response<string>>
    {
        public Guid UserId { get; set; }
    }

    public class RemoveSessionCommandHandler : IRequestHandler<RemoveSessionCommand, Response<string>>
    {
        private readonly IRepositoryAsync<LogSession> _repositoryAsync;

        public RemoveSessionCommandHandler(IRepositoryAsync<LogSession> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(RemoveSessionCommand request, CancellationToken cancellationToken)
        {
            var logSession = await _repositoryAsync.FirstOrDefaultAsync(new FilterSesionActiveSpecification(request.UserId, null));
            if (logSession == null)
            {
                throw new KeyNotFoundException($"No se encontro ninguna sesión activa");
            }
            logSession.IsActive = false;
            await _repositoryAsync.UpdateAsync(logSession);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>("Sesión cerrada correctamente", "Cerrada correctamente");
        }
    }
}
