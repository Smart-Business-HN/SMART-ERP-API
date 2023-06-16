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

namespace SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.UpdateOpportunityActivityCommand
{
    public class UpdateOpportunityActivityCommand : IRequest<Response<OpportunityActivityDto>>
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TypeActivityId { get; set; }
        public int StatusId { get; set; }
        public Guid UserId { get; set; }
        public int OpportunityId { get; set; }
    }

    public class UpdateOpportunityActivityCommandHandler : IRequestHandler<UpdateOpportunityActivityCommand, Response<OpportunityActivityDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityActivity> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IRepositoryHNAsync<Client> _clientRepositoryAsync;
        private readonly IRepositoryAsync<Opportunity> _oportunityRepositoryAsync;
        private readonly IRepositoryAsync<TypeActivity> _typeActivityRepositoryAsync;
        private readonly IGoogleCalendarService _googleCalendarService;

        public UpdateOpportunityActivityCommandHandler(
            IMapper mapper,
            IRepositoryAsync<OpportunityActivity> repositoryAsync,
            IJwtService jwtService,
            IRepositoryHNAsync<Client> clientRepositoryAsync,
            IRepositoryAsync<Opportunity> oportunityRepositoryAsync,
            IRepositoryAsync<TypeActivity> typeActivityRepositoryAsync,
            IGoogleCalendarService googleCalendarService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _clientRepositoryAsync = clientRepositoryAsync;
            _oportunityRepositoryAsync = oportunityRepositoryAsync;
            _typeActivityRepositoryAsync = typeActivityRepositoryAsync;
            _googleCalendarService = googleCalendarService;
        }

        public async Task<Response<OpportunityActivityDto>> Handle(UpdateOpportunityActivityCommand request, CancellationToken cancellationToken)
        {
            string title = "";
            string customerName = "";

            var getOpportunity = await _oportunityRepositoryAsync.FirstOrDefaultAsync(new GetOpportunityCustomerSpecification(request.OpportunityId));
            if (getOpportunity != null)
            {
                var client = await _clientRepositoryAsync.GetByIdAsync(getOpportunity.Customer!.MasterId);
                if (client != null)
                {
                    customerName = client.FullName;
                }
            }

            var getTypeActivity = await _typeActivityRepositoryAsync.GetByIdAsync(request.TypeActivityId);
            if (getTypeActivity == null)
                throw new ApiException($"No se encontro el tipo de actividad con el id: {request.TypeActivityId}");

            var opportunityActivity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (opportunityActivity == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            opportunityActivity.Description = request.Description;
            opportunityActivity.InitDate = request.InitDate;
            opportunityActivity.EndDate = request.EndDate;
            opportunityActivity.StatusId = request.StatusId;
            opportunityActivity.TypeActivityId = request.TypeActivityId;
            opportunityActivity.ModificationDate = DateTime.Now;
            opportunityActivity.ModificatedBy = _jwtService.GetSubjectToken();

            if (getTypeActivity.Name == "Correo")
                title = $"Envio de correo electrónico al cliente {customerName}";
            else if (getTypeActivity.Name == "Llamada")
                title = $"Llamada al cliente {customerName}";
            else if (getTypeActivity.Name == "Visita")
                title = $"Visita al cliente {customerName}";
            else if (getTypeActivity.Name == "Videollamada")
                title = $"Videollamada al cliente {customerName}";

            var getCalendar = _googleCalendarService.GetCalendarServiceAsync();
            await _googleCalendarService.UpdateEventAsync(getCalendar, opportunityActivity.GCEventId!, request.InitDate, request.EndDate, title, request.Description);

            await _repositoryAsync.UpdateAsync(opportunityActivity);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<OpportunityActivityDto>(opportunityActivity);
            return new Response<OpportunityActivityDto>(dto, message: $"Actividad actualizada correctamente");
        }
    }
}
