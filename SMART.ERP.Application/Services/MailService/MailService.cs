using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Domain.Settings;

namespace SMART.ERP.Application.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequestDto mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            if (mailRequest.ToCCEmail != null)
                email.Cc.Add(MailboxAddress.Parse(mailRequest.ToCCEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();

            if (mailRequest.Attachment != null)
            {
                byte[] fileByteArray;
                foreach (var file in mailRequest.Attachment)
                {
                    if (file.Length > 0)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            fileByteArray = memoryStream.ToArray();
                        }
                        if (file.ContentType == "application/pdf")
                        {
                            builder.Attachments.Add(file.FileName, fileByteArray, ContentType.Parse("application/pdf"));
                        }
                        else
                        {
                            if (file.ContentType == "image/png")
                            {
                                builder.Attachments.Add(file.FileName, fileByteArray, ContentType.Parse("image/png"));
                            }
                            else
                            {
                                builder.Attachments.Add(file.FileName, fileByteArray, ContentType.Parse("image/jpeg"));
                            }
                        }

                    }
                }

            }
            var companyImage = builder.LinkedResources.Add("Assets/company-image.png");
            var hikvisionImage = builder.LinkedResources.Add("Assets/hikvision-logo.png");
            var whatsappImage = builder.LinkedResources.Add("Assets/whatsapp.png");
            var fbImage = builder.LinkedResources.Add("Assets/facebook.png");
            var instagramImage = builder.LinkedResources.Add("Assets/instagram.png");
            var ubiquitiImage = builder.LinkedResources.Add("Assets/ubiquiti-logo.png");
            companyImage.ContentId = MimeUtils.GenerateMessageId();
            hikvisionImage.ContentId = MimeUtils.GenerateMessageId();
            whatsappImage.ContentId = MimeUtils.GenerateMessageId();
            fbImage.ContentId = MimeUtils.GenerateMessageId();
            instagramImage.ContentId = MimeUtils.GenerateMessageId();
            ubiquitiImage.ContentId = MimeUtils.GenerateMessageId();
            string firstDiv = @"
            @media only screen and (max-width:768px){
                .pad-div{padding:0px;}
                .parentContainer{width:100%;background-color:white;height:100%;}
                .firstContainer{width:100%;background-color:white;}
                .platinoText{font-size:10px;color:gray;}
            } 

            @media only screen and (min-width:769px){
                .pad-div{padding:3rem;}
                .parentContainer{width:100%;background-color:#888A8F;height:100%;}
                .firstContainer{width:60%;background-color:white;height:100%;margin:auto;font-family:'Poppins',sans-serif;border:solid 1px gray}
                .platinoText{font-size:14px;color:gray;}
            }
            .ql-align-right{
                text-align:right;
            }
            .ql-align-left{
                text-align:left;
            }
            .ql-align-justify{
                text-align:justify;
            }
            ";
            builder.HtmlBody = string.Format($@"
                <html>
                <head>
                <meta name=""viewport"" content=""width=device-width"">
                <style>
                    @import url('https://fonts.googleapis.com/css2?family=Poppins&display=swap');
                </style>
                <style>
                    {{0}}
                </style>
                </head>
                <body>
                <div class=""parentContainer"">
                    <div class=""firstContainer"">
                        <div class=""pad-div"">
                            <div style=""display:flex;"">                             
                                <img width=""40%"" height=""40%"" src=""cid:{{1}}"">              
                                <div style=""margin-left:auto;margin-top:auto;margin-bottom:auto"">
                                    <div style=""text-align:center;color:gray;"">
                                        Siguenos en:
                                        <span>
                                            <a href="""" style=""color:gray;text-decoration:none;"">
                                                <img height=""24px"" width=""24px"" src=""cid:{{2}}"">
                                            </a>
                                            </span>
                                            |
                                            <span>
                                                <a href=""https://www.facebook.com/SmartBusiness504/"" style=""color:gray;text-decoration:none;"">
                                                    <img height=""24px"" width=""24px"" src=""cid:{{3}}"">
                                                </a>
                                            </span>
                                            |
                                            <span>
                                            <a href=""https://www.instagram.com/smartbusiness504/?hl=es"" style=""color:gray;text-decoration:none;"">
                                                <img height=""24px"" width=""24px"" src=""cid:{{4}}"">
                                            </a>
                                        </span>
                                    </div>
                                    <div class=""platinoText"" style=""text-align:center"">
                                        www.smartbusiness.site
                                    </div>
                                </div>
                            </div>
                            <div style=""margin-top: 3rem;height:64%;margin-bottom:20px"">
                                {mailRequest.Body}
                            </div>
                        </div>
                        <div style=""display:flex;width:100%;padding-bottom:15px"">
                            <div style=""width:100%;background-image:linear-gradient(90deg, rgba(25,35,45,1) 0%, rgba(9,9,121,1) 27%, rgba(0,116,208,1) 100%);text-align: center; color: white; padding: 10px;"">
                                <span style=""display:inline-block;vertical-align:middle;margin-right:20px;"">
                                    Distribuidores autorizados
                                </span>
                                <span style=""display:flex; gap:4rem; justify-content: center; "">
                                    <img style=""width:150px;"" src=""cid:{{5}}"">
                                    <img style=""width:150px;"" src=""cid:{{6}}"">
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
</body>
</html>", firstDiv, companyImage.ContentId, whatsappImage.ContentId, fbImage.ContentId, instagramImage.ContentId, hikvisionImage.ContentId, ubiquitiImage.ContentId);
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
