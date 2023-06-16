using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MessageFeature.Commands.SendMessageCommand
{
    public class SendMessageCommand : IRequest<Response<string>>
    {
        public string Subject { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public Guid ReceptorId { get; set; }
        public List<IFormFile>? Attachments { get; set; }
    }

    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IMailService _mailService;

        public SendMessageCommandHandler(IRepositoryAsync<Prospect> repositoryAsync, IMailService mailService)
        {
            _repositoryAsync = repositoryAsync;
            _mailService = mailService;
        }

        public async Task<Response<string>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var checkProspect = await _repositoryAsync.GetByIdAsync(request.ReceptorId);
            if (checkProspect == null)
            {
                throw new KeyNotFoundException($"No se encontro el prospecto con id {request.ReceptorId}");
            }
            if (checkProspect.Email is null)
            {
                throw new ApiException("No se puede enviar correos a un prospecto sin correo electronico.");
            }
            var newMail = new MailRequestDto();
            newMail.Subject = request.Subject;
            newMail.Body = request.MessageContent;
            newMail.ToEmail = checkProspect.Email!;
            newMail.Attachment = request.Attachments;

            await _mailService.SendEmailAsync(newMail);

            return new Response<string>("Correo enviado con éxito");
        }
    }
}
