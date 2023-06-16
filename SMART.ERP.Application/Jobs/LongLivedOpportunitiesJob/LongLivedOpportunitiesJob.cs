using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Domain.Entities;
using Quartz;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.DTOs.Notification;

namespace SMART.ERP.Application.Jobs.LongLivedOpportunitiesJob
{
    public class LongLivedOpportunitiesJob : IJob
    {
        public static readonly JobKey Key = new JobKey("long-lived-opportunities-job", "jobs");
        private readonly IRepositoryAsync<OpportunitySchedules> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _opportunityRepositoryAsync;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IMapper _mapper;

        public LongLivedOpportunitiesJob(IRepositoryAsync<OpportunitySchedules> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Opportunity> opportunityRepositoryAsync, IHubContext<NotificationHub> hubContext,
            IRepositoryAsync<Notification> notificationRepositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _opportunityRepositoryAsync = opportunityRepositoryAsync;
            _hubContext = hubContext;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _mapper = mapper;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var managers = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Manager", null));
                var schedules = await _repositoryAsync.ListAsync();
                foreach (var schedule in schedules)
                {
                    var manager = managers.FirstOrDefault(x => x.Id == schedule.UserId);
                    if (manager != null)
                    {
                        var warningOpportunities = await _opportunityRepositoryAsync.ListAsync(new FilterActiveLongLivedOpportunitiesSpecification(schedule.OpportunityAge, manager.BranchOfficeId));
                        if (warningOpportunities.Count > 0)
                        {
                            var notificationLog = new List<Notification>();
                            foreach (var opportunity in warningOpportunities)
                            {
                                var newNotification = new Notification();
                                newNotification.Title = "Alerta de Oportunidad Longeva";
                                newNotification.UseRouter = true;
                                newNotification.Link = "/crm/opportunity/" + opportunity.Id;
                                newNotification.Description = $"La oportunidad {opportunity.Code} lleva más de {schedule.OpportunityAge} meses sin cerrar. Verifica los detalles.";
                                newNotification.Icon = "mat_outline:warning_amber";
                                newNotification.Time = DateTime.Now;
                                newNotification.Read = false;
                                newNotification.UserId = manager.Id;

                                notificationLog.Add(newNotification);
                            }
                            if (notificationLog.Count > 0)
                            {
                                var response = await _notificationRepositoryAsync.AddRangeAsync(notificationLog);
                                await _notificationRepositoryAsync.SaveChangesAsync();

                                foreach (var notification in response)
                                {
                                    var dto = _mapper.Map<NotificationDto>(notification);
                                    await _hubContext.Clients.User(manager.FullName).SendAsync("NewNotification", dto);
                                }
                            }
                        }
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
