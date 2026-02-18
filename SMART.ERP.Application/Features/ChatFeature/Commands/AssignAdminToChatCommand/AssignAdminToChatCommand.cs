using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.AssignAdminToChatCommand
{
    public class AssignAdminToChatCommand : IRequest<Response<string>>
    {
        public int ChatSessionId { get; set; }
        public Guid AdminUserId { get; set; }
    }

    public class AssignAdminToChatCommandHandler : IRequestHandler<AssignAdminToChatCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ChatSession> _chatSessionRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IHubContext<ChatHub> _chatHub;

        public AssignAdminToChatCommandHandler(
            IRepositoryAsync<ChatSession> chatSessionRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IHubContext<ChatHub> chatHub)
        {
            _chatSessionRepositoryAsync = chatSessionRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _chatHub = chatHub;
        }

        public async Task<Response<string>> Handle(AssignAdminToChatCommand request, CancellationToken cancellationToken)
        {
            var session = await _chatSessionRepositoryAsync.GetByIdAsync(request.ChatSessionId);
            if (session == null)
            {
                throw new KeyNotFoundException("No se encontró la sesión de chat.");
            }

            var admin = await _userRepositoryAsync.GetByIdAsync(request.AdminUserId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No se encontró el usuario administrador.");
            }

            session.AssignedAdminUserId = request.AdminUserId;
            session.Status = 1;

            await _chatSessionRepositoryAsync.UpdateAsync(session);
            await _chatSessionRepositoryAsync.SaveChangesAsync();

            await _chatHub.Clients.Group($"chat_{session.SessionIdentifier}").SendAsync("AdminJoined", new
            {
                AdminName = admin.FullName
            });

            await _chatHub.Clients.Group("admin_chat_listeners").SendAsync("SessionStatusChanged", new
            {
                Id = session.Id,
                session.SessionIdentifier,
                Status = 1,
                AdminName = admin.FullName
            });

            return new Response<string>($"Administrador {admin.FullName} asignado correctamente", "Administrador asignado correctamente");
        }
    }
}
