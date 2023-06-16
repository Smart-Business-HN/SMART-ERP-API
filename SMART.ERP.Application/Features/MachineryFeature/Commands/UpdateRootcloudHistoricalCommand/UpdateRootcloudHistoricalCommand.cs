using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.UpdateRootcloudHistoricalCommand
{
    public class UpdateRootcloudHistoricalCommand : IRequest<Response<MachineryRootcloudHistorical>>
    {
        public int Id { get; set; }
    }

    public class UpdateRootcloudHistoricalCommandHandler : IRequestHandler<UpdateRootcloudHistoricalCommand, Response<MachineryRootcloudHistorical>>
    {
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _repositoryAsync;
        private readonly IRootcloudHistoricalService _rootcloudHistoricalService;
        private readonly IRepositoryAsync<Machinery> _machineryRepositoryAsync;
        private readonly IRootcloudSessionService _rootcloudSessionService;

        public UpdateRootcloudHistoricalCommandHandler(IRepositoryAsync<MachineryRootcloudHistorical> repositoryAsync,
            IRootcloudHistoricalService rootcloudHistoricalService, IRepositoryAsync<Machinery> machineryRepositoryAsync, IRootcloudSessionService rootcloudSessionService)
        {
            _repositoryAsync = repositoryAsync;
            _rootcloudHistoricalService = rootcloudHistoricalService;
            _machineryRepositoryAsync = machineryRepositoryAsync;
            _rootcloudSessionService = rootcloudSessionService;
        }

        public async Task<Response<MachineryRootcloudHistorical>> Handle(UpdateRootcloudHistoricalCommand request, CancellationToken cancellationToken)
        {
            var machineryRootcloud = await _repositoryAsync.GetByIdAsync(request.Id);
            if (machineryRootcloud == null)
                throw new ApiException($"No se encontro ningun registro con el id {request.Id}");

            var machinery = await _machineryRepositoryAsync.FirstOrDefaultAsync(
                new FilterRootcloudHistoricalByIdSpecification(machineryRootcloud.MachineryId));
            if (machinery == null)
                throw new ApiException($"No se encontro ninguna maquina con el id {machineryRootcloud.MachineryId}");

            if (!machinery.IsRootcloudActive)
                throw new ApiException($"No se puede actualizar los datos por que este equipo no tiene Rootcloud");

            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                throw new ApiException("La sesion con los servicios de rootcloud expiro!! vuelva a intentarlo");

            machineryRootcloud = await _rootcloudHistoricalService.UpdateMachineryRootcloudHistorical(machineryRootcloud, machinery, session);

            await _repositoryAsync.UpdateAsync(machineryRootcloud);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<MachineryRootcloudHistorical>(machineryRootcloud);
        }
    }
}
