using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.CloseChatSessionCommand
{
    public class CloseChatSessionCommand : IRequest<Response<string>>
    {
        public int ChatSessionId { get; set; }
    }

    public class CloseChatSessionCommandHandler : IRequestHandler<CloseChatSessionCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ChatSession> _chatSessionRepositoryAsync;
        private readonly IHubContext<ChatHub> _chatHub;

        public CloseChatSessionCommandHandler(
            IRepositoryAsync<ChatSession> chatSessionRepositoryAsync,
            IHubContext<ChatHub> chatHub)
        {
            _chatSessionRepositoryAsync = chatSessionRepositoryAsync;
            _chatHub = chatHub;
        }

        public async Task<Response<string>> Handle(CloseChatSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _chatSessionRepositoryAsync.GetByIdAsync(request.ChatSessionId);
            if (session == null)
            {
                throw new KeyNotFoundException("No se encontró la sesión de chat.");
            }

            session.Status = 2;
            session.ClosedAt = DateTime.UtcNow;

            await _chatSessionRepositoryAsync.UpdateAsync(session);
            await _chatSessionRepositoryAsync.SaveChangesAsync();

            await _chatHub.Clients.Group($"chat_{session.SessionIdentifier}").SendAsync("SessionClosed");

            await _chatHub.Clients.Group("admin_chat_listeners").SendAsync("SessionStatusChanged", new
            {
                Id = session.Id,
                session.SessionIdentifier,
                Status = 2
            });

            return new Response<string>("Sesión de chat cerrada correctamente", "Sesión cerrada correctamente");
        }
    }
}
