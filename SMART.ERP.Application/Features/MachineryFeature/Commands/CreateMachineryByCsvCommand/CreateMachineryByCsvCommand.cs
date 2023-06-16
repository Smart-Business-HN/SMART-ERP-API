using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Http;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.Services.Rootcloud;
using System.Text;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.CreateMachineryByCsvCommand
{
    public class CreateMachineryByCsvCommand : IRequest<Response<bool>>
    {
        public IFormFile ExcelFile { get; set; } = null!;
    }

    public class CreateMachineryByCsvCommandHandler : IRequestHandler<CreateMachineryByCsvCommand, Response<bool>>
    {
        private readonly IRepositoryAsync<Machinery> _repositoryAsync;
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _rootcloudRepositoryAsync;
        private readonly IRootcloudHistoricalService _rootcloudHistoricalService;
        private readonly IRepositoryAsync<Subcategory> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;
        private readonly IRepositoryAsync<MachineryFailureReport> _machineryFailureRepositoryAsync;
        private readonly IRepositoryAsync<MachineryFailure> _failureRepositoryAsync;

        public CreateMachineryByCsvCommandHandler(IRepositoryAsync<Machinery> repositoryAsync,
            IRepositoryAsync<MachineryRootcloudHistorical> rootcloudRepositoryAsync,
            IRootcloudHistoricalService rootcloudHistoricalService,
            IRepositoryAsync<Subcategory> categoryRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync,
            IRepositoryAsync<MachineryFailureReport> machineryFailureRepositoryAsync,
            IRepositoryAsync<MachineryFailure> failureRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _rootcloudRepositoryAsync = rootcloudRepositoryAsync;
            _rootcloudHistoricalService = rootcloudHistoricalService;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
            _machineryFailureRepositoryAsync = machineryFailureRepositoryAsync;
            _failureRepositoryAsync = failureRepositoryAsync;
        }
        public async Task<Response<bool>> Handle(CreateMachineryByCsvCommand request, CancellationToken cancellationToken)
        {
            var newMachinery = new Machinery();
            var listMachineries = await _repositoryAsync.ListAsync();
            var subCategories = await _categoryRepositoryAsync.ListAsync();
            int duplicates = 0;
            if (subCategories.Count == 0)
            {
                throw new ApiException($"No se encontraron categorias");
            }

            var getStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(
                new FilterStatusFromNameSpecification("Operativo"));
            if (getStatus == null)
                throw new ApiException($"Ocurrio un problema al asignar el estado");

            var getFailure = await _failureRepositoryAsync.FirstOrDefaultAsync(
                new FilterMachineryFailureSpecification("Sin falla", null));
            if (getFailure == null)
                throw new ApiException($"Ocurrio un problema al asignar el estado");

            List<MachineryCsvDto> machineriesCsv = new List<MachineryCsvDto>();
            List<MachineryRootcloudHistorical> rootcloudHistoricals = new List<MachineryRootcloudHistorical>();
            List<MachineryFailureReport> machineryFailureReports = new List<MachineryFailureReport>();
            var test = "Nota: Los datos asignados a cada máquina en esta plantilla puede ser editados después de subir el archivo al sistema, si el equipo aún no " +
                "presenta un cliente deberá asignar un valor por defecto según su criterio como ser: Distribuidora Platino, para el campo de acuerdo de mantenimiento " +
                "deberá escribir la palabra: Si o No, también mencionar que todos los datos son requeridos para poder registrar estos equipos en sistema. " +
                "Para la columna de estado deberá escribir algunos de los siguientes según su criterio: Activo, Inactivo o Retirado";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                request.ExcelFile.CopyTo(stream);
                stream.Position = 0;
                var reader = ExcelReaderFactory.CreateReader(stream);
                if (reader.RowCount > 11)
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(0) != null && reader.GetValue(0).ToString() != test)
                        {
                            if (reader.GetValue(0) != null)
                            {
                                var item = new MachineryCsvDto();
                                item.DeviceName = reader.GetValue(0).ToString();
                                item.SerialNum = reader.GetValue(1).ToString();
                                item.Country = reader.GetValue(2).ToString();
                                item.Province = reader.GetValue(3).ToString();
                                item.Customer = reader.GetValue(4).ToString();
                                item.ActiveMaintenance = reader.GetValue(5).ToString();
                                item.Interval = reader.GetValue(6).ToString();
                                item.InitialMaintenance = reader.GetValue(7).ToString();
                                item.Subcategory = reader.GetValue(8).ToString();
                                item.Status = reader.GetValue(9).ToString();
                                machineriesCsv.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    throw new ApiException($"El archivo debe contener al menos más de un registro");
                }
            }
            duplicates = machineriesCsv.Count - machineriesCsv.DistinctBy(x => x.SerialNum).ToList().Count;
            machineriesCsv = machineriesCsv.DistinctBy(x => x.SerialNum).Skip(1).ToList();
            foreach (var item in machineriesCsv)
            {
                var checkSerialNum = listMachineries.Find(x => x.SerialNum == item.SerialNum);
                if (checkSerialNum != null)
                    throw new ApiException($"Ya existe una maquina con el numero de serie: {checkSerialNum.SerialNum}");

                var subcategory = subCategories.First(x => x.Name == item.Subcategory);
                if (subcategory == null)
                    throw new ApiException($"No se encontro ninguna categoria con el nombre: {item.Subcategory}");

                var machinery = new Machinery
                {
                    Interval = Convert.ToDecimal(item.Interval),
                    InitialMaintenance = Convert.ToDecimal(item.InitialMaintenance),
                    DeviceName = item.DeviceName,
                    SerialNum = item.SerialNum,
                    Customer = item.Customer,
                    Province = item.Province,
                    Country = item.Country,
                    ActiveMaintenance = item.ActiveMaintenance.ToLower() == "Si" ? true : false,
                    IsRootcloudActive = false,
                    TenantId = "N/A",
                    BaseInfoId = "N/A",
                    CatName = "N/A",
                    ModelType = "N/A",
                    MachineTypeName = "N/A",
                    MachineTypeId = "N/A",
                    MachineCategoryId = 0,
                    CreateDate = DateTime.Now,
                    SubcategoryId = subcategory.Id,
                };

                newMachinery = await _repositoryAsync.AddAsync(machinery);
                await _repositoryAsync.SaveChangesAsync();

                var historical = new MachineryRootcloudHistorical();
                historical.MachineryId = machinery.Id;
                historical.CreationDate = DateTime.Now;
                historical.Hourmeter = 0;
                historical.NextMaintenance = newMachinery.InitialMaintenance;
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

                historical = _rootcloudHistoricalService.AssingNextMaintenance(historical, newMachinery);

                rootcloudHistoricals.Add(historical);

                var failure = new MachineryFailureReport();
                failure.CreationDate = DateTime.Now;
                failure.CreatedBy = "Sistema";
                failure.StatusId = getStatus.Id;
                failure.MachineryFailureId = getFailure.Id;
                failure.Description = "Sin falla";
                failure.MachineryId = newMachinery.Id;

                machineryFailureReports.Add(failure);
            }

            if (rootcloudHistoricals.Count > 0)
            {
                await _rootcloudRepositoryAsync.AddRangeAsync(rootcloudHistoricals);
                await _rootcloudRepositoryAsync.SaveChangesAsync();
            }

            if (machineryFailureReports.Count > 0)
            {
                await _machineryFailureRepositoryAsync.AddRangeAsync(machineryFailureReports);
                await _machineryFailureRepositoryAsync.SaveChangesAsync();
            }

            return new Response<bool>(true, $"Se registro toda la maquinaria, elementos registrados: {machineriesCsv.Count}," +
                $" elementos duplicados: {duplicates}");

        }
    }
}
