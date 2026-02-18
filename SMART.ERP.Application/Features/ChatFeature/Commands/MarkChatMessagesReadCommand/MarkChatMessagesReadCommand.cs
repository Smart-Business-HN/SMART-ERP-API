using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ChatSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.MarkChatMessagesReadCommand
{
    public class MarkChatMessagesReadCommand : IRequest<Response<string>>
    {
        public int ChatSessionId { get; set; }
        public string ReaderType { get; set; } = null!;
    }

    public class MarkChatMessagesReadCommandHandler : IRequestHandler<MarkChatMessagesReadCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ChatMessage> _chatMessageRepositoryAsync;
        private readonly IRepositoryAsync<ChatSession> _chatSessionRepositoryAsync;

        public MarkChatMessagesReadCommandHandler(
            IRepositoryAsync<ChatMessage> chatMessageRepositoryAsync,
            IRepositoryAsync<ChatSession> chatSessionRepositoryAsync)
        {
            _chatMessageRepositoryAsync = chatMessageRepositoryAsync;
            _chatSessionRepositoryAsync = chatSessionRepositoryAsync;
        }

        public async Task<Response<string>> Handle(MarkChatMessagesReadCommand request, CancellationToken cancellationToken)
        {
            var messages = await _chatMessageRepositoryAsync.ListAsync(
                new FilterChatMessagesBySessionIdSpecification(request.ChatSessionId));

            var oppositeSenderType = request.ReaderType == "admin" ? "visitor" : "admin";

            var unreadMessages = messages.Where(m => !m.IsRead && m.SenderType == oppositeSenderType).ToList();

            foreach (var message in unreadMessages)
            {
                var trackedMessage = await _chatMessageRepositoryAsync.GetByIdAsync(message.Id);
                if (trackedMessage != null)
                {
                    trackedMessage.IsRead = true;
                    await _chatMessageRepositoryAsync.UpdateAsync(trackedMessage);
                }
            }

            await _chatMessageRepositoryAsync.SaveChangesAsync();

            if (request.ReaderType == "admin")
            {
                var session = await _chatSessionRepositoryAsync.GetByIdAsync(request.ChatSessionId);
                if (session != null)
                {
                    session.UnreadAdminCount = 0;
                    await _chatSessionRepositoryAsync.UpdateAsync(session);
                    await _chatSessionRepositoryAsync.SaveChangesAsync();
                }
            }

            return new Response<string>("Mensajes marcados como leídos", "Operación exitosa");
        }
    }
}
