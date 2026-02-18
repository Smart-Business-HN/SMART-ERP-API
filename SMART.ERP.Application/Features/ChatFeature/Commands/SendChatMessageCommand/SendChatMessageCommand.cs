using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Chat;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.SendChatMessageCommand
{
    public class SendChatMessageCommand : IRequest<Response<ChatMessageDto>>
    {
        public int ChatSessionId { get; set; }
        public string Content { get; set; } = null!;
        public string SenderType { get; set; } = null!;
        public Guid? SenderAdminUserId { get; set; }
        public string SenderName { get; set; } = null!;
        public string SessionIdentifier { get; set; } = null!;
    }

    public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, Response<ChatMessageDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<ChatMessage> _chatMessageRepositoryAsync;
        private readonly IRepositoryAsync<ChatSession> _chatSessionRepositoryAsync;
        private readonly IHubContext<ChatHub> _chatHub;

        public SendChatMessageCommandHandler(IMapper mapper,
            IRepositoryAsync<ChatMessage> chatMessageRepositoryAsync,
            IRepositoryAsync<ChatSession> chatSessionRepositoryAsync,
            IHubContext<ChatHub> chatHub)
        {
            _mapper = mapper;
            _chatMessageRepositoryAsync = chatMessageRepositoryAsync;
            _chatSessionRepositoryAsync = chatSessionRepositoryAsync;
            _chatHub = chatHub;
        }

        public async Task<Response<ChatMessageDto>> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
        {
            var newMessage = new ChatMessage
            {
                ChatSessionId = request.ChatSessionId,
                Content = request.Content,
                SenderType = request.SenderType,
                SenderAdminUserId = request.SenderAdminUserId,
                SenderName = request.SenderName,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _chatMessageRepositoryAsync.AddAsync(newMessage);
            await _chatMessageRepositoryAsync.SaveChangesAsync();

            var session = await _chatSessionRepositoryAsync.GetByIdAsync(request.ChatSessionId);
            if (session != null)
            {
                session.LastMessagePreview = request.Content.Length > 200
                    ? request.Content.Substring(0, 200)
                    : request.Content;
                session.LastMessageAt = DateTime.UtcNow;

                if (request.SenderType == "visitor")
                {
                    session.UnreadAdminCount++;
                }

                await _chatSessionRepositoryAsync.UpdateAsync(session);
                await _chatSessionRepositoryAsync.SaveChangesAsync();
            }

            var dto = _mapper.Map<ChatMessageDto>(newMessage);

            await _chatHub.Clients.Group($"chat_{request.SessionIdentifier}").SendAsync("ReceiveMessage", dto);

            if (request.SenderType == "visitor")
            {
                await _chatHub.Clients.Group("admin_chat_listeners").SendAsync("SessionUpdated", new
                {
                    request.SessionIdentifier,
                    LastMessage = dto
                });
            }

            return new Response<ChatMessageDto>(dto, "Mensaje enviado correctamente");
        }
    }
}
