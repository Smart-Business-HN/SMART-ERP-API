using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.AdvisorGoalSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.AdvisorGoal;
using System.Globalization;

namespace SMART.ERP.Application.Features.AdvisorGoalFeature.Commands.UpdateAdvisorGoalCommand
{
    public class UpdateAdvisorGoalCommand : IRequest<Response<string>>
    {
        public Guid UserId { get; set; }
        public List<UpdateAdvisorGoalDto> Goals { get; set; } = null!;
    }

    public class UpdateAdvisorGoalCommandHandler : IRequestHandler<UpdateAdvisorGoalCommand, Response<string>>
    {
        private readonly IRepositoryAsync<AdvisorGoal> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public UpdateAdvisorGoalCommandHandler(IRepositoryAsync<AdvisorGoal> repositoryAsync, IMapper mapper, IHubContext<NotificationHub> hubContext,
            IRepositoryAsync<Notification> notificationRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _hubContext = hubContext;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }
        public async Task<Response<string>> Handle(UpdateAdvisorGoalCommand request, CancellationToken cancellationToken)
        {
            var currentDate = DateTime.Now;
            //if (currentDate.Day > 3)
            //{
            //    throw new ApiException("El tiempo limite para actualizar metas ha expirado");
            //}
            var getUser = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (getUser == null)
            {
                throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
            }
            var advisorGoalList = await _repositoryAsync.ListAsync(new FilterAdvisorGoalByYearSpecification(DateTime.Now.Year, request.UserId));
            foreach (var goal in request.Goals)
            {
                var checkAdvisorGoal = advisorGoalList.FirstOrDefault(x => x.InitDate.Month == goal.Date.Month);
                if (checkAdvisorGoal != null)
                {
                    if (checkAdvisorGoal.Enabled != true)
                    {
                        throw new ApiException($"La meta del mes {goal.Date.Month} esta bloqueada");
                    }
                }
            }

            var toUpdateList = new List<AdvisorGoal>();
            var toAddList = new List<AdvisorGoal>();
            foreach (var goal in request.Goals)
            {
                var checkAdvisorGoal = advisorGoalList.FirstOrDefault(x => x.InitDate.Month == goal.Date.Month);
                if (checkAdvisorGoal != null)
                {
                    checkAdvisorGoal.Goal = goal.Goal;
                    toUpdateList.Add(checkAdvisorGoal);
                }
                else
                {
                    var newGoal = new AdvisorGoal();
                    newGoal.UserId = request.UserId;
                    newGoal.Enabled = true;
                    newGoal.Goal = goal.Goal;
                    if (goal.Date.Month == currentDate.Month)
                    {
                        newGoal.InitDate = currentDate;
                        newGoal.EndDate = DateTime.Now.AddMonths(1);
                    }
                    else
                    {
                        newGoal.InitDate = goal.Date;
                        newGoal.EndDate = goal.Date.AddMonths(1);
                    }
                    toAddList.Add(newGoal);
                }
            }

            if (toUpdateList.Count > 0)
            {
                await _repositoryAsync.UpdateRangeAsync(toUpdateList);
                await _repositoryAsync.SaveChangesAsync();
            }
            if (toAddList.Count > 0)
            {
                await _repositoryAsync.AddRangeAsync(toAddList);
                await _repositoryAsync.SaveChangesAsync();
            }

            if (toUpdateList.Any(x => x.InitDate.Month == currentDate.Month) || toAddList.Any(x => x.InitDate.Month == currentDate.Month))
            {
                var goal = toUpdateList.FirstOrDefault(x => x.InitDate.Month == currentDate.Month);
                if (goal == null)
                {
                    goal = toAddList.FirstOrDefault(x => x.InitDate.Month == currentDate.Month);
                }
                var notification = new Notification();
                notification.Title = "Actualizacion meta mensual";
                notification.UseRouter = false;
                notification.Time = currentDate;
                notification.Link = null;
                notification.Read = false;
                notification.Icon = "mat_outline:calendar_today";
                notification.UserId = request.UserId;
                notification.Description = $"Tu meta mensual ha sido actualizada. La meta de este mes es de <b>${goal!.Goal.ToString("N2", CultureInfo.CurrentCulture)}</b>";

                var notificationResponse = await _notificationRepositoryAsync.AddAsync(notification);
                await _notificationRepositoryAsync.SaveChangesAsync();

                var notificationDto = _mapper.Map<NotificationDto>(notificationResponse);

                await _hubContext.Clients.User(getUser!.FullName).SendAsync("NewNotification", notificationDto);
            }

            return new Response<string>("Actualizado correctamente");
        }
    }
}
