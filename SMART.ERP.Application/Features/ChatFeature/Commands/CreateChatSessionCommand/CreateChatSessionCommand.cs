using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Chat;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.ChatSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.CreateChatSessionCommand
{
    public class CreateChatSessionCommand : IRequest<Response<ChatSessionDto>>
    {
        public string SessionIdentifier { get; set; } = null!;
        public string VisitorName { get; set; } = null!;
        public string? VisitorEmail { get; set; }
        public bool IsAuthenticated { get; set; }
        public Guid? EcommerceUserId { get; set; }
    }

    public class CreateChatSessionCommandHandler : IRequestHandler<CreateChatSessionCommand, Response<ChatSessionDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ChatSession> _chatSessionRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public CreateChatSessionCommandHandler(IMapper mapper,
            IRepositoryAsync<ChatSession> chatSessionRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Notification> notificationRepositoryAsync,
            IHubContext<ChatHub> chatHub,
            IHubContext<NotificationHub> notificationHub)
        {
            _mapper = mapper;
            _chatSessionRepositoryAsync = chatSessionRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _chatHub = chatHub;
            _notificationHub = notificationHub;
        }

        public async Task<Response<ChatSessionDto>> Handle(CreateChatSessionCommand request, CancellationToken cancellationToken)
        {
            var existingSession = await _chatSessionRepositoryAsync.FirstOrDefaultAsync(
                new FilterChatSessionByIdentifierSpecification(request.SessionIdentifier));

            if (existingSession != null)
            {
                var existingDto = _mapper.Map<ChatSessionDto>(existingSession);
                return new Response<ChatSessionDto>(existingDto, "Sesión de chat existente");
            }

            var newSession = new ChatSession
            {
                SessionIdentifier = request.SessionIdentifier,
                VisitorName = request.VisitorName,
                VisitorEmail = request.VisitorEmail,
                IsAuthenticated = request.IsAuthenticated,
                EcommerceUserId = request.EcommerceUserId,
                Status = 0,
                CreatedAt = DateTime.UtcNow,
                UnreadAdminCount = 0
            };

            await _chatSessionRepositoryAsync.AddAsync(newSession);
            await _chatSessionRepositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<ChatSessionDto>(newSession);

            await _chatHub.Clients.Group("admin_chat_listeners").SendAsync("NewChatSession", dto);

            var superAdmins = await _userRepositoryAsync.ListAsync(
                new FilterUserByRoleSpecification("SuperAdmin", null));

            if (superAdmins.Count > 0)
            {
                foreach (var admin in superAdmins)
                {
                    var notification = new Notification();
                    notification.Title = "Nuevo chat de soporte";
                    notification.Icon = "heroicons_outline:chat-bubble-left-right";
                    notification.Description = $"{request.VisitorName} ha iniciado un chat de soporte";
                    notification.Time = DateTime.Now;
                    notification.Read = false;
                    notification.UserId = admin.Id;
                    notification.Link = "/crm/chat";
                    notification.UseRouter = true;

                    var response = await _notificationRepositoryAsync.AddAsync(notification);
                    await _notificationRepositoryAsync.SaveChangesAsync();
                    var notificationDto = _mapper.Map<NotificationDto>(response);

                    await _notificationHub.Clients.User(admin.FullName).SendAsync("NewNotification", notificationDto);
                }
            }

            return new Response<ChatSessionDto>(dto, "Sesión de chat creada correctamente");
        }
    }
}
