using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMaintenanceCommand
{
    public class CreateMaintenanceCommand : IRequest<Response<MachineryRootcloudHistorical>>
    {
        public string Person { get; set; } = null!;
        public string? UrlDocument { get; set; }
        public decimal Hourmeter { get; set; }
        public string? Observation { get; set; }
        public DateTime CreationDate { get; set; }
        public int MachineryId { get; set; }
        public int HistoricalId { get; set; }
    }

    public class MaintenanceCommandHandler : IRequestHandler<CreateMaintenanceCommand, Response<MachineryRootcloudHistorical>>
    {
        private readonly IRepositoryAsync<MachineryMaintenance> _repositoryAsync;
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _rootcloudHistoricalRepositoryAsync;
        private readonly IRepositoryAsync<Machinery> _machineryRepositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public MaintenanceCommandHandler(IRepositoryAsync<MachineryMaintenance> repositoryAsync,
            IRepositoryAsync<MachineryRootcloudHistorical> rootcloudHistoricalRepositoryAsync,
            IRepositoryAsync<Machinery> machineryRepositoryAsync,
            IJwtService jwtService, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _rootcloudHistoricalRepositoryAsync = rootcloudHistoricalRepositoryAsync;
            _machineryRepositoryAsync = machineryRepositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<MachineryRootcloudHistorical>> Handle(CreateMaintenanceCommand request, CancellationToken cancellationToken)
        {
            var machinery = await _machineryRepositoryAsync.GetByIdAsync(request.MachineryId, cancellationToken);
            if (machinery == null)
                throw new ApiException($"No se encontro la maquina con el id: {request.MachineryId}");
            var newRecord = _mapper.Map<MachineryMaintenance>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var response = await UpdateMaintenanceDone(request.HistoricalId, machinery, request.Hourmeter);
            return new Response<MachineryRootcloudHistorical>(response, message: $"Mantenimiento registrado exitosamente");
        }

        public async Task<MachineryRootcloudHistorical> UpdateMaintenanceDone(int historicalId, Machinery machinery, decimal maintenanceHourMeter)
        {
            var rootcloudHistorical = await _rootcloudHistoricalRepositoryAsync.GetByIdAsync(historicalId);
            if (rootcloudHistorical == null)
                throw new ApiException($"No se encontro ningun registro de Rootcloud");

            rootcloudHistorical.MaintenanceDone = true;
            rootcloudHistorical.LateHours = 0;

            if (maintenanceHourMeter > rootcloudHistorical.Hourmeter)
            {
                rootcloudHistorical.Hourmeter = maintenanceHourMeter;
            }
            else
            {
                //decimal diference = rootcloudHistorical.Hourmeter - maintenanceHourMeter;
                //if (diference > 60)
                //{
                //    throw new ApiException($"El horometro asiganado en en mantenimiento tiene una " +
                //        $"diferencia negativa mayor a 60 horas, procure registrar los mantenimientos en tiempo y forma");
                //}
            }

            rootcloudHistorical.NextMaintenance = maintenanceHourMeter + machinery.Interval;
            rootcloudHistorical.MissingForNextMaintenance = 0;
            rootcloudHistorical.LastMaintenance = maintenanceHourMeter;
            rootcloudHistorical.LateHours = rootcloudHistorical.NextMaintenance - rootcloudHistorical.Hourmeter;

            await _rootcloudHistoricalRepositoryAsync.UpdateAsync(rootcloudHistorical);
            await _rootcloudHistoricalRepositoryAsync.SaveChangesAsync();

            return rootcloudHistorical;
        }
    }
}
