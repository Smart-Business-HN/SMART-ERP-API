using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.GoogleCalendarService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.MASTER.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.CreateOpportunityActivityCommand
{
    public class CreateOpportunityActivityCommand : IRequest<Response<OpportunityActivityDto>>
    {
        public string Description { get; set; } = null!;
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TypeActivityId { get; set; }
        public int StatusId { get; set; }
        public Guid UserId { get; set; }
        public int OpportunityId { get; set; }
    }

    public class CreateOpportunityActivityCommandHandler : IRequestHandler<CreateOpportunityActivityCommand, Response<OpportunityActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryHNAsync<Client> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _oportunityRepositoryAsync;
        private readonly IRepositoryAsync<TypeActivity> _typeActivityRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IGoogleCalendarService _googleCalendarService;

        public CreateOpportunityActivityCommandHandler(
            IMapper mapper,
            IRepositoryAsync<OpportunityActivity> repositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync,
            IRepositoryHNAsync<Client> clientRepositoryAsync,
            IRepositoryAsync<Opportunity> oportunityRepositoryAsync,
            IRepositoryAsync<TypeActivity> typeActivityRepositoryAsync,
            IJwtService jwtService,
            IGoogleCalendarService googleCalendarService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _clientRepositoryAsync = clientRepositoryAsync;
            _oportunityRepositoryAsync = oportunityRepositoryAsync;
            _typeActivityRepositoryAsync = typeActivityRepositoryAsync;
            _jwtService = jwtService;
            _googleCalendarService = googleCalendarService;
        }

        public async Task<Response<OpportunityActivityDto>> Handle(CreateOpportunityActivityCommand request, CancellationToken cancellationToken)
        {
            string title = "";
            string customerName = "";

            var getUser = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (getUser == null)
                throw new ApiException($"No se encontro el usuario con el id: {request.UserId}");

            var getTypeActivity = await _typeActivityRepositoryAsync.GetByIdAsync(request.TypeActivityId);
            if (getTypeActivity == null)
                throw new ApiException($"No se encontro el tipo de actividad con el id: {request.TypeActivityId}");

            var getStatus = await _statusRepositoryAsync.GetByIdAsync(request.StatusId);
            if (getStatus == null)
                throw new ApiException($"No se encontro el estado con el id: {request.StatusId}");

            var getOpportunity = await _oportunityRepositoryAsync.FirstOrDefaultAsync(new GetOpportunityCustomerSpecification(request.OpportunityId));
            if (getOpportunity != null)
            {
                var client = await _clientRepositoryAsync.GetByIdAsync(getOpportunity.Customer!.MasterId);
                if (client != null)
                {
                    customerName = client.FullName;
                }
            }

            if (getTypeActivity.Name == "Correo")
                title = $"Envio de correo electrónico al cliente {customerName}";
            else if (getTypeActivity.Name == "Llamada")
                title = $"Llamada al cliente {customerName}";
            else if (getTypeActivity.Name == "Visita")
                title = $"Visita al cliente {customerName}";
            else if (getTypeActivity.Name == "Videollamada")
                title = $"Videollamada al cliente {customerName}";

            var newRecord = _mapper.Map<OpportunityActivity>(request);
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            newRecord.CreationDate = DateTime.Now;

            var getCalendar = _googleCalendarService.GetCalendarServiceAsync();
            var calendarEvent = await _googleCalendarService.CreateEventAsync(getCalendar, getUser!.Email, newRecord.InitDate, newRecord.EndDate, title, newRecord.Description);
            newRecord.GCEventId = calendarEvent.Id;

            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            data.Status = getStatus;
            data.User = getUser;
            data.TypeActivity = getTypeActivity;
            var dto = _mapper.Map<OpportunityActivityDto>(data);
            return new Response<OpportunityActivityDto>(dto, message: $"Actividad creada exitosamente");
        }
    }
}
