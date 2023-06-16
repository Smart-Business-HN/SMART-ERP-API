using SMART.ERP.Application.DTOs.Mail;

namespace SMART.ERP.Application.Services.MailService
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestDto mailRequest);
    }
}
