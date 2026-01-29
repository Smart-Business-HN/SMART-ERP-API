using Microsoft.AspNetCore.SignalR;
using Quartz;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Jobs.AdvisorGoalJob
{
    public class AdvisorGoalJob : IJob
    {
        public static readonly JobKey Key = new JobKey("advisor-goal-job", "jobs");
        private readonly IRepositoryAsync<AdvisorGoal> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;

        public AdvisorGoalJob(IRepositoryAsync<AdvisorGoal> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IHubContext<NotificationHub> hubContext,
            IRepositoryAsync<Notification> notificationRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _hubContext = hubContext;
            _notificationRepositoryAsync = notificationRepositoryAsync;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var currentDate = DateTime.Now;
                var advisorListPastMonth = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByMonthSpecification(null, null, DateTime.Now.AddMonths(-1)));
                var advisorList = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByMonthSpecification(null, null, currentDate));
                var missingAdvisors = advisorListPastMonth.FindAll(x => !advisorList.Exists(y => y.UserId == x.UserId));
                var toAddAdvisor = new List<AdvisorGoal>();
                foreach (var advisor in missingAdvisors)
                {
                    var newAdvisor = new AdvisorGoal();
                    newAdvisor.UserId = advisor.UserId;
                    newAdvisor.Enabled = false;
                    newAdvisor.Goal = advisor.Goal;
                    newAdvisor.InitDate = currentDate;
                    newAdvisor.EndDate = DateTime.Now.AddMonths(1);
                    toAddAdvisor.Add(newAdvisor);
                }
                if (toAddAdvisor.Count > 0)
                {
                    await _repositoryAsync.AddRangeAsync(toAddAdvisor);
                    await _repositoryAsync.SaveChangesAsync();
                }
                foreach (var item in advisorList)
                {
                    item.Enabled = false;
                }
                await _repositoryAsync.UpdateRangeAsync(advisorList);
                await _repositoryAsync.SaveChangesAsync();
                if (advisorList.Count > 0 || missingAdvisors.Count > 0)
                {
                    var managers = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Manager", null));
                    var notificationList = new List<Notification>();
                    foreach (var manager in managers)
                    {
                        var notification = new Notification();
                        notification.Time = DateTime.Now;
                        notification.Title = "Actualizacion de metas";
                        notification.Description = "Se han actualizado las metas de este mes para tus asesores.";
                        notification.Icon = "heroicons_outline:adjustments";
                        notification.UserId = manager.Id;
                        notificationList.Add(notification);
                        await _hubContext.Clients.User(manager.FullName).SendAsync("NewNotification", notification);
                    }
                    if (notificationList.Count > 0)
                    {
                        await _notificationRepositoryAsync.AddRangeAsync(notificationList);
                        await _notificationRepositoryAsync.SaveChangesAsync();
                    }
                }

                return;
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de lanzar el evento advisor-job-goal");
            }
        }
    }
}
