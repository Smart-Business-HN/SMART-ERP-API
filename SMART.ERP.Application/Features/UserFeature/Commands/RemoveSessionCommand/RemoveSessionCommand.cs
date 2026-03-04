using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.RefreshTokenSpecification;
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
        private readonly IRepositoryAsync<RefreshToken> _refreshTokenRepositoryAsync;

        public RemoveSessionCommandHandler(
            IRepositoryAsync<LogSession> repositoryAsync,
            IRepositoryAsync<RefreshToken> refreshTokenRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _refreshTokenRepositoryAsync = refreshTokenRepositoryAsync;
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

            var activeRefreshTokens = await _refreshTokenRepositoryAsync.ListAsync(
                new FilterRefreshTokenByUserSpecification(request.UserId));
            foreach (var token in activeRefreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedDate = DateTime.UtcNow;
                token.RevokedReason = "Cierre de sesión";
                await _refreshTokenRepositoryAsync.UpdateAsync(token);
            }
            if (activeRefreshTokens.Count > 0)
                await _refreshTokenRepositoryAsync.SaveChangesAsync();

            return new Response<string>("Sesión cerrada correctamente", "Cerrada correctamente");
        }
    }
}
