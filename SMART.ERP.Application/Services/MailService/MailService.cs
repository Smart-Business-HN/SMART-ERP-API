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
            var platinoImage = builder.LinkedResources.Add("Assets/pm-logo.png");
            var sanyImage = builder.LinkedResources.Add("Assets/sany-logo.png");
            var whatsappImage = builder.LinkedResources.Add("Assets/whatsapp.png");
            var fbImage = builder.LinkedResources.Add("Assets/facebook.png");
            var instagramImage = builder.LinkedResources.Add("Assets/instagram.png");
            platinoImage.ContentId = MimeUtils.GenerateMessageId();
            sanyImage.ContentId = MimeUtils.GenerateMessageId();
            whatsappImage.ContentId = MimeUtils.GenerateMessageId();
            fbImage.ContentId = MimeUtils.GenerateMessageId();
            instagramImage.ContentId = MimeUtils.GenerateMessageId();
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
                .platinoText{font-size:20px;color:gray;}
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
                                <img width=""30%"" height=""30%"" src=""cid:{{1}}"">              
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
                                                <a href=""https://www.facebook.com/platinomotors/"" style=""color:gray;text-decoration:none;"">
                                                    <img height=""24px"" width=""24px"" src=""cid:{{3}}"">
                                                </a>
                                            </span>
                                            |
                                            <span>
                                            <a href=""https://www.instagram.com/platinomotors/?hl=es"" style=""color:gray;text-decoration:none;"">
                                                <img height=""24px"" width=""24px"" src=""cid:{{4}}"">
                                            </a>
                                        </span>
                                    </div>
                                    <div class=""platinoText"" style=""text-align:center"">
                                        w w w . p l a t i n o . h n
                                    </div>
                                </div>
                            </div>
                            <div style=""margin-top: 3rem;height:64%;margin-bottom:20px"">
                                {mailRequest.Body}
                            </div>
                        </div>
                        <div style=""display:flex;width:100%;padding-bottom:15px"">
                            <div style=""background-color:#050C44;width:25%;height:inherit;"">
                
                            </div>
                            <div style=""width:75%;background-image:linear-gradient(to right,#B50D21,#F72F26);text-align: center; color: white; padding: 10px;"">
                                <span style=""display:inline-block;vertical-align:middle;margin-right:20px;"">
                                    Distribuidores exclusivos
                                </span>
                                <span style=""display:inline-block; vertical-align:middle"">
                                    <img style=""width:180px;"" src=""cid:{{5}}"">
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
</body>
</html>", firstDiv, platinoImage.ContentId, whatsappImage.ContentId, fbImage.ContentId, instagramImage.ContentId, sanyImage.ContentId);
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
