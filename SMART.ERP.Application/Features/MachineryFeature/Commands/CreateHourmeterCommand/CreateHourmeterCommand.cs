using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateHourmeterCommand
{
    public class CreateHourmeterCommand : IRequest<Response<MachineryRootcloudHistorical>>
    {
        public int MachineryId { get; set; }
        public decimal Hourmeter { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class CreateHourmeterCommandHandler : IRequestHandler<CreateHourmeterCommand, Response<MachineryRootcloudHistorical>>
    {
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _repositoryAsync;
        private readonly IRepositoryAsync<Machinery> _machineryRepositoryAsync;
        private readonly IRootcloudHistoricalService _rootcloudHistoricalService;

        public CreateHourmeterCommandHandler(IRepositoryAsync<MachineryRootcloudHistorical> repositoryAsync,
            IRepositoryAsync<Machinery> machineryRepositoryAsync,
            IRootcloudHistoricalService rootcloudHistoricalService)
        {
            _repositoryAsync = repositoryAsync;
            _machineryRepositoryAsync = machineryRepositoryAsync;
            _rootcloudHistoricalService = rootcloudHistoricalService;
        }

        public async Task<Response<MachineryRootcloudHistorical>> Handle(CreateHourmeterCommand request, CancellationToken cancellationToken)
        {
            if (request.CreationDate.Date > DateTime.UtcNow.Date)
                throw new ApiException($"No se puede seleccionar una fecha mayor a la actual");

            var machinery = await _machineryRepositoryAsync.GetByIdAsync(request.MachineryId);
            if (machinery == null)
                throw new ApiException($"No se encontro niguna maquina con el id {request.MachineryId}");

            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(new CheckIfExistHistoricalSpecification(request.MachineryId, request.CreationDate));
            if (checkIfExist != null)
            {
                checkIfExist.Hourmeter = request.Hourmeter;
                checkIfExist = _rootcloudHistoricalService.AssingNextMaintenance(checkIfExist, machinery);
                await _repositoryAsync.UpdateAsync(checkIfExist);
                await _repositoryAsync.SaveChangesAsync();
                return new Response<MachineryRootcloudHistorical>(checkIfExist, "Horometro registrado correctamente");
            }
            else
            {
                var historical = new MachineryRootcloudHistorical();
                historical.MachineryId = machinery.Id;
                historical.Hourmeter = request.Hourmeter;
                historical.Lat = 0;
                historical.Lng = 0;
                historical.Status = "N/A";
                historical.Milenage = 0;
                historical.CreationDate = request.CreationDate;
                historical.FuelLevel = 0;
                historical.AverageFuelConsumption = 0;
                historical.RealtimeFuelConsumption = 0;
                historical.TotalFuelConsumption = 0;
                historical.TotalFuelUnit = "N/A";
                historical.TimestampLocal = "N/A";
                historical.IsSystemAlert = true;

                var checkHourmeter = await _repositoryAsync.FirstOrDefaultAsync(new GetEndRootcloudHistoricalSpecification(request.MachineryId));
                if (checkHourmeter != null)
                {
                    if (request.CreationDate.Date < checkHourmeter.CreationDate.Date)
                        throw new ApiException($"Ya existe un registro de horometro mayor a la fecha seleccionada");
                    if (checkHourmeter.Hourmeter > request.Hourmeter)
                        throw new ApiException($"No se puede asignar un horometro menor al actual");
                }

                historical = _rootcloudHistoricalService.AssingNextMaintenance(historical, machinery);

                var result = await _repositoryAsync.AddAsync(historical);
                await _repositoryAsync.SaveChangesAsync();
                return new Response<MachineryRootcloudHistorical>(result, "Horometro registrado correctamente");
            }
        }
    }
}
