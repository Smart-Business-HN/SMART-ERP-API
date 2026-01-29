using Quartz;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Specifications.LogSessionSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Jobs.LogSessionJob
{
    public class LogSessionJob : IJob
    {
        public static readonly JobKey LogJobKey = new JobKey("log-session-job", "jobs");
        private readonly IRepositoryAsync<LogSession> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMailService _mailService;

        public LogSessionJob(IRepositoryAsync<LogSession> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IMailService mailService)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _mailService = mailService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var salesAdvisors = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Sales Advisor", null));
                var managers = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Manager", null));
                var logSession = await _repositoryAsync.ListAsync(new FilterLogSessionFromTodaySpecification());

                foreach (var manager in managers)
                {
                    var managerAdvisors = salesAdvisors.FindAll(x => x.BranchOfficeId == manager.BranchOfficeId);
                    if (managerAdvisors.Count > 0)
                    {
                        MailRequestDto mail = new();
                        mail.Subject = "Log de Sesiones " + DateTime.Now.ToString("d", new System.Globalization.CultureInfo("es-MX"));
                        mail.ToEmail = manager.Email;
                        mail.Body = @"<table style=""width:100%;border-collapse:collapse;border:1px solid black;"">
<tr>
<th>Usuario</th>
<th>Sesiones</th>
</tr>";
                        foreach (var advisor in managerAdvisors)
                        {
                            mail.Body += $"<tr><td style=\"text-align:center;border:1px solid black;\">{advisor.FullName}</td><td style=\"text-align:center;border:1px solid black;\">{logSession.FindAll(x => x.UserId == advisor.Id).Count}</td></tr>";
                        }

                        mail.Body += "</table>";

                        await _mailService.SendEmailAsync(mail);
                    }
                }

                return;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
