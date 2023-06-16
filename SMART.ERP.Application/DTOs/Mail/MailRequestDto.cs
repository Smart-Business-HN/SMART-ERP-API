using Microsoft.AspNetCore.Http;

namespace SMART.ERP.Application.DTOs.Mail
{
    public class MailRequestDto
    {
        public string ToEmail { get; set; } = null!;
        public string? ToCCEmail { get; set; }
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string? FileName { get; set; }
        public List<IFormFile>? Attachment { get; set; }
    }
}
