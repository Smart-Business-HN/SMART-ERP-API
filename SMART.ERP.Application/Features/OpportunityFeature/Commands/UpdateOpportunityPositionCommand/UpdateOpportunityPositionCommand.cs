using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.UpdateOpportunityPositionCommand
{
    public class UpdateOpportunityPositionCommand : IRequest<Response<OpportunityDto>>
    {
        public int OpportunityId { get; set; }
        public int OpportunityStepId { get; set; }
        public int? ReasonId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Position { get; set; }
    }

    public class UpdateOpportunityPositionCommandHandler : IRequestHandler<UpdateOpportunityPositionCommand, Response<OpportunityDto>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IMailService _mailService;
        private readonly IJwtService _jwtService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IRepositoryAsync<LossReason> _lossRepositoryAsync;
        private readonly IRepositoryAsync<WinReason> _winRepositoryAsync;

        public UpdateOpportunityPositionCommandHandler(IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<OpportunityStep> stepRepositoryAsync, IMapper mapper,
            IRepositoryAsync<User> userRepositoryAsync, IMailService mailService, IJwtService jwtService, IHubContext<NotificationHub> notificationHub,
            IRepositoryAsync<Notification> notificationRepositoryAsync, IRepositoryAsync<LossReason> lossRepositoryAsync, IRepositoryAsync<WinReason> winRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _mapper = mapper;
            _userRepositoryAsync = userRepositoryAsync;
            _mailService = mailService;
            _jwtService = jwtService;
            _notificationHub = notificationHub;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _lossRepositoryAsync = lossRepositoryAsync;
            _winRepositoryAsync = winRepositoryAsync;
        }

        public async Task<Response<OpportunityDto>> Handle(UpdateOpportunityPositionCommand request, CancellationToken cancellationToken)
        {
            var requestUser = _jwtService.GetSubjectToken();
            var checkIfOpportunityExist = await _repositoryAsync.GetByIdAsync(request.OpportunityId);
            if (checkIfOpportunityExist == null)
            {
                throw new KeyNotFoundException($"No se encontro la oportunidad con Id {request.OpportunityId}");
            }

            var checkIfOpportunityStepExists = await _stepRepositoryAsync.GetByIdAsync(request.OpportunityStepId);
            if (checkIfOpportunityStepExists == null)
            {
                throw new KeyNotFoundException($"No se encontro el paso con id {request.OpportunityStepId}");
            }

            var opportunityUser = await _userRepositoryAsync.GetByIdAsync(checkIfOpportunityExist.UserId);
            if (opportunityUser == null)
            {
                throw new KeyNotFoundException("¡Ocurrio un error con el asesor de esta oportunidad!");
            }
            if (checkIfOpportunityStepExists.Name == "Ganado" || checkIfOpportunityStepExists.Name == "Perdido" || checkIfOpportunityStepExists.Name == "Abandonado")
            {
                if (checkIfOpportunityExist.QtyItems == 0)
                {
                    throw new ApiException("No es posible mover una oportunidad sin productos a este paso.");
                }
                if (request.ReasonId == null)
                {
                    throw new ApiException("Debes proveer una razón para este movimiento");
                }
                if (checkIfOpportunityStepExists.Name == "Ganado")
                {
                    var checkReason = await _winRepositoryAsync.GetByIdAsync((int)request.ReasonId);
                    if (checkReason == null)
                    {
                        throw new KeyNotFoundException($"No se encontro la razon con id {request.ReasonId}");
                    }
                    checkIfOpportunityExist.WinReasonId = request.ReasonId;
                    checkIfOpportunityExist.LossReasonId = null;
                }
                else
                {
                    var checkReason = await _lossRepositoryAsync.GetByIdAsync((int)request.ReasonId);
                    if (checkReason == null)
                    {
                        throw new KeyNotFoundException($"No se encontro la razon con id {request.ReasonId}");
                    }
                    checkIfOpportunityExist.LossReasonId = request.ReasonId;
                    checkIfOpportunityExist.WinReasonId = null;
                }
                if (checkIfOpportunityExist.OpportunityStepId != request.OpportunityStepId)
                {
                    checkIfOpportunityExist.ClosingDate = DateTime.Now;
                }

            }
            else
            {
                checkIfOpportunityExist.ClosingDate = null;
            }

            if (checkIfOpportunityExist.OpportunityStepId == request.OpportunityStepId)
            {
                var opportunityList = await _repositoryAsync.ListAsync(new UpdateOpportunityPositionSpecification(request.OpportunityStepId, null, opportunityUser!.Id, null, null));
                if (request.StartDate != null && request.EndDate != null)
                {
                    var filteredList = opportunityList.FindAll(x => x.CreationDate >= request.StartDate && x.CreationDate <= request.EndDate);
                    if (filteredList.Count > 0)
                    {
                        if (filteredList[request.Position - 1].Position != request.Position)
                        {
                            request.Position = filteredList[request.Position - 1].Position;
                        }
                    }
                }
                if (request.Position > checkIfOpportunityExist.Position)
                {
                    var toUpdateList = opportunityList.FindAll(x => x.Position > checkIfOpportunityExist.Position && x.Position <= request.Position);
                    if (toUpdateList.Count > 0)
                    {
                        foreach (var opportunity in toUpdateList)
                        {
                            opportunity.Position -= 1;
                        }
                        await _repositoryAsync.UpdateRangeAsync(toUpdateList);
                        await _repositoryAsync.SaveChangesAsync();
                    }
                }
                if (request.Position < checkIfOpportunityExist.Position)
                {
                    var toUpdateList = opportunityList.FindAll(x => x.Position >= request.Position && x.Position < checkIfOpportunityExist.Position);
                    foreach (var opportunity in toUpdateList)
                    {
                        opportunity.Position += 1;
                    }
                    await _repositoryAsync.UpdateRangeAsync(toUpdateList);
                    await _repositoryAsync.SaveChangesAsync();
                }
            }
            else
            {
                var previousContainer = await _repositoryAsync.ListAsync(new UpdateOpportunityPositionSpecification(checkIfOpportunityExist.OpportunityStepId, null, opportunityUser!.Id, null, null));
                var currentContainer = await _repositoryAsync.ListAsync(new UpdateOpportunityPositionSpecification(request.OpportunityStepId, null, opportunityUser!.Id, null, null));
                if (previousContainer.Count > 0)
                {
                    var toUpdateList = previousContainer.FindAll(x => x.Position > checkIfOpportunityExist.Position);
                    foreach (var opportunity in toUpdateList)
                    {
                        opportunity.Position -= 1;
                    }
                    await _repositoryAsync.UpdateRangeAsync(toUpdateList);
                    await _repositoryAsync.SaveChangesAsync();
                }
                if (currentContainer.Count > 0)
                {
                    if (request.StartDate != null && request.EndDate != null)
                    {
                        var filteredList = currentContainer.FindAll(x => x.CreationDate >= request.StartDate && x.CreationDate <= request.EndDate);
                        if (filteredList.Count > 0)
                        {
                            if (request.Position - 1 != filteredList.Count)
                            {
                                if (filteredList[request.Position - 1].Position != request.Position)
                                {
                                    request.Position = filteredList[request.Position - 1].Position;
                                }
                            }
                            else
                            {
                                request.Position = filteredList[filteredList.Count - 1].Position + 1;
                            }
                        }
                    }
                    var toUpdateList = currentContainer.FindAll(x => x.Position >= request.Position);
                    foreach (var opportunity in toUpdateList)
                    {
                        opportunity.Position += 1;
                    }
                    await _repositoryAsync.UpdateRangeAsync(toUpdateList);
                    await _repositoryAsync.SaveChangesAsync();
                }

            }



            checkIfOpportunityExist.OpportunityStepId = request.OpportunityStepId;
            checkIfOpportunityExist.Position = request.Position;

            await _repositoryAsync.UpdateAsync(checkIfOpportunityExist);
            await _repositoryAsync.SaveChangesAsync();

            var manager = await _userRepositoryAsync.FirstOrDefaultAsync(
                new FilterUserByRoleSpecification("Manager", opportunityUser!.BranchOfficeId));

            if (manager != null)
            {
                var message = new MailRequestDto();
                message.ToEmail = manager.Email;
                message.Subject = $"Actualizacion de la oportunidad {checkIfOpportunityExist.Code}";
                message.Body = $@"
                    Hola {manager.FullName},<br><br>

                    El usuario <b>{requestUser}</b> ha realizado el movimiento de la oportunidad <b>{checkIfOpportunityExist.Code}</b> hacia el paso ""<b>{checkIfOpportunityStepExists.Name}</b>"". Verifica que la información
                    de la oportunidad es correcta.
                    ";


                var notification = new Notification();
                notification.Title = "Movimiento de Oportunidad";
                notification.Icon = "heroicons_outline:information-circle";
                notification.Description = $"El usuario <b>{requestUser}</b> ha movido la oportunidad <b>{checkIfOpportunityExist.Code}</b> hacia el paso <b>{checkIfOpportunityStepExists.Name}</b>";
                notification.Time = DateTime.Now;
                notification.Read = false;
                notification.UserId = manager.Id;

                var response = await _notificationRepositoryAsync.AddAsync(notification);
                await _notificationRepositoryAsync.SaveChangesAsync();
                var notificationDto = _mapper.Map<NotificationDto>(response);

                await _mailService.SendEmailAsync(message);
                await _notificationHub.Clients.User(manager.FullName).SendAsync("NewNotification", notificationDto);
            }

            var dto = _mapper.Map<OpportunityDto>(checkIfOpportunityExist);

            return new Response<OpportunityDto>(dto, "Actualizado exitosamente");
        }
    }
}
