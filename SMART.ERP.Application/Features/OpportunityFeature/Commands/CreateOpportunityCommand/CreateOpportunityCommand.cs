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
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCommand
{
    public class CreateOpportunityCommand : IRequest<Response<OpportunityDto>>
    {
        public Guid CustomerId { get; set; }
        public int OpportunityStepId { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class CreateOpportunityCommandHandler : IRequestHandler<CreateOpportunityCommand, Response<OpportunityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IRepositoryAsync<Customer> _repositoryCustomerAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<InterestLevel> _interesteLevelRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly IRepositoryHNAsync<Client> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IMailService _mailService;

        public CreateOpportunityCommandHandler(IMapper mapper, IRepositoryAsync<Opportunity> repositoryAsync, IRepositoryAsync<Customer> repositoryCustomerAsync
            , IRepositoryAsync<OpportunityStep> stepRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<InterestLevel> interesteLevelRepositoryAsync,
            IJwtService jwtService, IHubContext<NotificationHub> notificationHub, IRepositoryHNAsync<Client> clientRepositoryAsync,
            IRepositoryAsync<Notification> notificationRepositoryAsync, IMailService mailService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _repositoryCustomerAsync = repositoryCustomerAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _interesteLevelRepositoryAsync = interesteLevelRepositoryAsync;
            _jwtService = jwtService;
            _notificationHub = notificationHub;
            _clientRepositoryAsync = clientRepositoryAsync;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _mailService = mailService;
        }

        public async Task<Response<OpportunityDto>> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
        {
            var checkIfStepExist = await _stepRepositoryAsync.GetByIdAsync(request.OpportunityStepId);
            if (checkIfStepExist == null)
            {
                throw new KeyNotFoundException($"No se encontro el paso con id {request.OpportunityStepId}");
            }

            if (request.CreationDate.Date > DateTime.Now.Date)
            {
                throw new ApiException("La fecha de inicio no debe ser mayor a la fecha de hoy.");
            }

            var client = await _repositoryCustomerAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.CustomerId));
            if (client == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente con id {request.CustomerId}");
            }
            var checkClientHN = await _clientRepositoryAsync.GetByIdAsync(request.CustomerId);
            if (checkClientHN == null)
            {
                throw new KeyNotFoundException($"Ocurrio un error con este cliente");
            }
            var uid = _jwtService.GetUidToken();
            var opportunityList = await _repositoryAsync.ListAsync(new FilterOpportunityByStepSpecification(request.OpportunityStepId, null, null, 1, uid));
            if (opportunityList.Count > 0)
            {
                foreach (var item in opportunityList)
                {
                    item.Position += 1;
                }
                await _repositoryAsync.UpdateRangeAsync(opportunityList);
                await _repositoryAsync.SaveChangesAsync();
            }

            var newRecord = _mapper.Map<Opportunity>(request);
            var interestlevel = await _interesteLevelRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Bajo", null));

            newRecord.CustomerId = client!.Id;
            newRecord.CreationDate = request.CreationDate;
            newRecord.Code = await GenerateCode(request.CreationDate);
            newRecord.IsActive = true;
            newRecord.UserId = uid;
            newRecord.Position = 1;
            newRecord.ProbabilityPercentage = 0;
            newRecord.Total = 0;
            newRecord.ApplyOnCredit = false;
            newRecord.Budget = 0;
            newRecord.InterestLevelId = interestlevel!.Id;

            var data = await _repositoryAsync.AddAsync(newRecord);
            data.User = await _userRepositoryAsync.GetByIdAsync(uid);
            data.InterestLevel = interestlevel;
            await _repositoryAsync.SaveChangesAsync();

            var assignedUser = await _userRepositoryAsync.GetByIdAsync(newRecord.UserId);
            var manager = await _userRepositoryAsync.FirstOrDefaultAsync(new FilterUserByRoleSpecification("Manager", assignedUser!.BranchOfficeId));
            if (manager != null)
            {
                var mailRequest = new MailRequestDto();
                mailRequest.Subject = "Nueva oportunidad";
                mailRequest.ToEmail = manager.Email;
                mailRequest.Body = $@"Hola {manager.FullName}!<br><br>
El asesor {assignedUser!.FullName} ha creado una nueva oportunidad. Puedes consultar la informacion de la oportunidad desde el codigo {data.Code} o ingresando 
directamente al enlace: https://adminpm.platino.hn/#/crm/opportunity/{data.Id}. Recuerda que puedes consultar la informacion del cliente desde el panel administrativo.";

                var managerNotification = new Notification();
                managerNotification.Title = "Nueva oportunidad";
                managerNotification.Icon = "heroicons_outline:information-circle";
                managerNotification.Description = $"El asesor <b>{assignedUser!.FullName}</b> ha creado la oportunidad {newRecord.Code}. Verifica la informacion del cliente.";
                managerNotification.Time = DateTime.Now;
                managerNotification.UseRouter = true;
                managerNotification.Link = "/crm/opportunity/" + data.Id;
                managerNotification.Read = false;
                managerNotification.UserId = manager.Id;

                var response = await _notificationRepositoryAsync.AddAsync(managerNotification);
                await _notificationRepositoryAsync.SaveChangesAsync();

                var managerNotificationDto = _mapper.Map<NotificationDto>(response);

                await _mailService.SendEmailAsync(mailRequest);
                await _notificationHub.Clients.User(manager.FullName).SendAsync("NewNotification", managerNotificationDto);
            }

            var dto = _mapper.Map<OpportunityDto>(data);

            return new Response<OpportunityDto>(dto, message: $"Oportunidad creada exitosamente con codigo: {dto.Code}");

        }

        public async Task<string> GenerateCode(DateTime date)
        {
            var getLastRegister = await _repositoryAsync.ListAsync(new FilterLastOpportunitySpecification());
            return CodeIdentity.OPM + "-" + date.Year.ToString() + "-" + (getLastRegister.Count > 0 ? getLastRegister[0].Id + 1 : 1);
        }
    }
}
