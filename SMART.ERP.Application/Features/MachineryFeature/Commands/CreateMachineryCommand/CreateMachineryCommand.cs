using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMachineryCommand
{
    public class CreateMachineryCommand : IRequest<Response<MachineryNoListObjectsDto>>
    {
        public string DeviceName { get; set; } = null!;
        public string SerialNum { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string MachineTypeName { get; set; } = null!;
        public bool ActiveMaintenance { get; set; }
        public int Interval { get; set; }
        public int InitialMaintenance { get; set; }
        public int SubcategoryId { get; set; }
        public int BrandId { get; set; }
        public string? Status { get; set; }
    }

    public class CreateMachineryCommandHandler : IRequestHandler<CreateMachineryCommand, Response<MachineryNoListObjectsDto>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _rootcloudRepositoryAsync;
        private readonly IRootcloudHistoricalService _rootcloudHistoricalService;
        private readonly IRepositoryAsync<Subcategory> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<MachineryFailureReport> _machineryFailureRepositoryAsync;
        private readonly IRepositoryAsync<MachineryFailure> _failureRepositoryAsync;
        private readonly IRepositoryAsync<Brand> _brandRepositoryAsync;

        public CreateMachineryCommandHandler(IRepositoryAsync<Machinery> repositoryAsync, IMapper mapper,
            IRepositoryAsync<MachineryRootcloudHistorical> rootcloudRepositoryAsync,
            IRootcloudHistoricalService rootcloudHistoricalService,
            IRepositoryAsync<Subcategory> categoryRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync,
            IRepositoryAsync<MachineryFailureReport> machineryFailureRepositoryAsync,
            IRepositoryAsync<MachineryFailure> failureRepositoryAsync,
            IRepositoryAsync<Brand> brandRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _rootcloudRepositoryAsync = rootcloudRepositoryAsync;
            _rootcloudHistoricalService = rootcloudHistoricalService;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _machineryFailureRepositoryAsync = machineryFailureRepositoryAsync;
            _failureRepositoryAsync = failureRepositoryAsync;
            _brandRepositoryAsync = brandRepositoryAsync;
        }
        public async Task<Response<MachineryNoListObjectsDto>> Handle(CreateMachineryCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistBySerialNum = await _repositoryAsync.FirstOrDefaultAsync(new FilterBySerialNumMachinerySpecification(request.SerialNum, null));
            if (checkIfExistBySerialNum != null)
                throw new ApiException($"Ya existe una maquina con el numero de serie: {request.SerialNum}");

            var getStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(
                new FilterStatusFromNameSpecification("Operativo"));
            if (getStatus == null)
                throw new ApiException($"Ocurrio un problema al asignar el estado");

            var getCategory = await _categoryRepositoryAsync.GetByIdAsync(request.SubcategoryId);
            if (getCategory == null)
                throw new ApiException($"No se encontro ninguna categoria con el id: {request.SubcategoryId}");

            var getFailure = await _failureRepositoryAsync.FirstOrDefaultAsync(
                new FilterMachineryFailureSpecification("Sin falla", null));
            if (getFailure == null)
                throw new ApiException($"Ocurrio un problema al asignar el estado");

            var getBrand = await _brandRepositoryAsync.GetByIdAsync(request.BrandId);
            if (getBrand == null)
                throw new ApiException($"No se encontro ninguna marca con el id: {request.BrandId}");

            var newRecord = _mapper.Map<Machinery>(request);
            newRecord.CreateDate = DateTime.Now;
            newRecord.BaseInfoId = "N/A";
            newRecord.ModelType = "N/A";
            newRecord.MachineTypeId = "N/A";
            newRecord.MachineTypeName = "N/A";
            newRecord.TenantId = "N/A";
            newRecord.CatName = "N/A";
            newRecord.InitialMaintenance = request.InitialMaintenance;
            newRecord.Interval = request.Interval;
            newRecord.MachineCategoryId = 0;
            newRecord.SubcategoryId = request.SubcategoryId;
            newRecord.IsRootcloudActive = false;
            newRecord.Status = request.Status == MachineryStatus.Activo.ToString()
                || request.Status == MachineryStatus.Inactivo.ToString()
                || request.Status == MachineryStatus.Retirado.ToString()
                ? request.Status : "N/A";

            var machinery = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();

            var historical = new MachineryRootcloudHistorical();
            historical.MachineryId = machinery.Id;
            historical.CreationDate = DateTime.Now;
            historical.Hourmeter = 0;
            historical.NextMaintenance = request.InitialMaintenance;
            historical.Lat = 0;
            historical.Lng = 0;
            historical.Status = "N/A";
            historical.Milenage = 0;
            historical.FuelLevel = 0;
            historical.AverageFuelConsumption = 0;
            historical.RealtimeFuelConsumption = 0;
            historical.TotalFuelConsumption = 0;
            historical.TotalFuelUnit = "N/A";
            historical.TimestampLocal = "N/A";
            historical.IsSystemAlert = true;

            historical = _rootcloudHistoricalService.AssingNextMaintenance(historical, machinery);

            await _rootcloudRepositoryAsync.AddAsync(historical);
            await _rootcloudRepositoryAsync.SaveChangesAsync();

            var failure = new MachineryFailureReport();
            failure.CreationDate = DateTime.Now;
            failure.CreatedBy = "Sistema";
            failure.StatusId = getStatus.Id;
            failure.MachineryFailureId = getFailure.Id;
            failure.Description = "Sin falla";
            failure.MachineryId = machinery.Id;
            await _machineryFailureRepositoryAsync.AddAsync(failure);
            await _machineryFailureRepositoryAsync.SaveChangesAsync();

            var dto = _mapper.Map<MachineryNoListObjectsDto>(machinery);
            return new Response<MachineryNoListObjectsDto>(dto, "Maquina registrada correctamente");
        }
    }
}
