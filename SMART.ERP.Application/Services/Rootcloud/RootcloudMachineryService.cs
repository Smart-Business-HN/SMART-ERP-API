using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using RestSharp;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using System.Net;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public class RootcloudMachineryService : IRootcloudMachineryService
    {
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _historicalRepositoryAsync;
        private readonly IRepositoryAsync<Machinery> _machineryRepositoryAsync;
        private readonly IRootcloudSessionService _rootcloudSessionService;
        private readonly IRootcloudHistoricalService _rootcloudHistoricalService;
        private readonly IRepositoryAsync<MachineryFailure> _failureRepositoryAsync;
        private readonly IRepositoryAsync<MachineryFailureReport> _reportFailureRepositoryAsync;
        private readonly IRepositoryAsync<Status> _statusRepositoryAsync;

        public RootcloudMachineryService(
            IRepositoryAsync<MachineryRootcloudHistorical> historicalRepositoryAsync,
            IRepositoryAsync<Machinery> machineryRepositoryAsync,
            IRootcloudSessionService rootcloudSessionService,
            IRootcloudHistoricalService rootcloudHistoricalService,
            IRepositoryAsync<MachineryFailure> failureRepositoryAsync,
            IRepositoryAsync<MachineryFailureReport> reportFailureRepositoryAsync,
            IRepositoryAsync<Status> statusRepositoryAsync)
        {
            _historicalRepositoryAsync = historicalRepositoryAsync;
            _machineryRepositoryAsync = machineryRepositoryAsync;
            _rootcloudSessionService = rootcloudSessionService;
            _rootcloudHistoricalService = rootcloudHistoricalService;
            _failureRepositoryAsync = failureRepositoryAsync;
            _reportFailureRepositoryAsync = reportFailureRepositoryAsync;
            _statusRepositoryAsync = statusRepositoryAsync;
        }

        public async Task<List<DeviceDto>> GetAllMachineries()
        {
            await _rootcloudSessionService.RemoveSession();
            await _rootcloudSessionService.Login();

            await _rootcloudSessionService.RemoveSession();
            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                await GetAllMachineries();

            var url = $"{RootcloudStaticConfig.baseUri}/user-management/device/getDeviceRegisterList";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("language", "es-ES");
            request.AddHeader("time_zone", 8);
            request.AddHeader("access-token", session.TicketId);

            List<GetDeviceTargetRequest> targetList = new List<GetDeviceTargetRequest>();
            var target = new GetDeviceTargetRequest()
            {
                Id = 1,
                Type = "machine_category"
            };
            targetList.Add(target);
            var body = new GetDeviceRequest()
            {
                OrgId = session.OrgIds,
                QueryOrgIds = session.OrgIds,
                Page = 1,
                Limit = 400,
                TargetId = targetList,
            };
            var json = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            request.AddParameter("application/json", json, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            HttpStatusCode statuscode = response.StatusCode;
            int numericStatusCode = (int)statuscode;
            if (numericStatusCode != 200 && response.Content == null)
            {
                throw new ApiException("Rootcloud error: Ocurrio un error al obtener esta informacion");
            }

            var result = JsonConvert.DeserializeObject<ResponseDto<List<DeviceDto>>>(response.Content.ToString());
            if (result == null)
                throw new ApiException("Rootcloud error: la respuesta a esta solicitud no pudo ser mapeada");
            if (result.Msg!.ToLower().Contains("error")
                || result.Msg.ToLower().Contains("ApiException"))
            {
                throw new ApiException(result.Msg);
            }

            if (result.Data == null)
            {
                throw new ApiException(result.Msg);
            }

            return result.Data;
        }

        public async Task<bool> CreateOrUpdateMachinery(List<DeviceDto> devices)
        {
            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                await CreateOrUpdateMachinery(devices);

            var getStatus = await _statusRepositoryAsync.FirstOrDefaultAsync(
                new FilterStatusFromNameSpecification("Operativo"));
            if (getStatus == null)
                throw new ApiException($"Ocurrio un problema al asignar el estado");

            var getFailure = await _failureRepositoryAsync.FirstOrDefaultAsync(
                new FilterMachineryFailureSpecification("Sin falla", null));
            if (getFailure == null)
                throw new ApiException($"Ocurrio un problema al asignar el estado");

            foreach (var item in devices)
            {
                var machinery = await _machineryRepositoryAsync.FirstOrDefaultAsync(new CheckIfExistMachinerySpecification(item.SerialNum));
                if (machinery == null)
                {
                    var newRecord = new Machinery();
                    newRecord.BaseInfoId = item.BaseInfoId;
                    newRecord.DeviceName = item.DeviceName;
                    newRecord.SerialNum = item.SerialNum;
                    newRecord.ModelType = item.ModelType;
                    newRecord.MachineTypeId = item.MachineTypeId;
                    newRecord.MachineTypeName = item.MachineTypeName;
                    newRecord.Country = item.Country;
                    newRecord.Province = item.Province;
                    newRecord.CreateDate = DateTime.Parse(item.CreateDate);
                    newRecord.TenantId = item.TenantId;
                    newRecord.Customer = item.Customer == null ? "Sin Asignar" : item.Customer;
                    newRecord.CatName = item.CatName;
                    newRecord.MachineCategoryId = item.MachineCategoryId;
                    newRecord.ActiveMaintenance = true;
                    newRecord.IsRootcloudActive = true;
                    newRecord.Status = MachineryStatus.Inactivo.ToString();

                    var historical = await _rootcloudHistoricalService.HistoricalWorkingConditions(item.BaseInfoId, item.ModelType, DateTime.Now.AddDays(-200), DateTime.Now, 1, 10, session);
                    if (historical.Data == null)
                    {
                        throw new ApiException($"No se encontro datos de Rootcloud en los ultimos 90 dias para la maquina con numero de serie: {item.SerialNum}");
                    }

                    if (historical.Data.Count > 0)
                    {
                        var selectFirst = historical.Data.First();
                        var machineryHistorical = new MachineryRootcloudHistorical();
                        machineryHistorical.Lat = selectFirst.GPS_Latitude;
                        machineryHistorical.Lng = selectFirst.GPS_Longitude;
                        machineryHistorical.Status = selectFirst.DeviceStatus;
                        machineryHistorical.Hourmeter = RootcloudStaticConfig.WorkingHour(selectFirst);
                        machineryHistorical.Milenage = selectFirst.Mileage > 0 ? selectFirst.Mileage
                            : selectFirst.Total_mileage > 0 ? selectFirst.Total_mileage : 0;
                        machineryHistorical.TimestampLocal = selectFirst.Timestamp_Local == null ? DateTime.Now.ToString() : selectFirst.Timestamp_Local;
                        machineryHistorical.FuelLevel = selectFirst.Fuel_level;
                        machineryHistorical.AverageFuelConsumption = selectFirst.Average_fuel_consumption;
                        machineryHistorical.RealtimeFuelConsumption = selectFirst.Realtime_fuel_consumption;
                        machineryHistorical.TotalFuelConsumption = selectFirst.Total_fuel_consumption;
                        machineryHistorical.TotalFuelUnit = RootcloudStaticConfig.UnitOfMeasurementRootcloud(int.Parse(item.ModelType));

                        DateTime checkEndDate = DateTime.Parse(selectFirst.Timestamp_Local);
                        if (checkEndDate.Date < DateTime.Now.Date)
                        {
                            double diffOfDates = (DateTime.Now.Date - checkEndDate.Date).TotalDays;
                            if (diffOfDates > 5)
                            {
                                machineryHistorical.SystemRunning = false;
                            }
                        }
                        else
                        {
                            machineryHistorical.SystemRunning = true;
                        }

                        var intervals = RootcloudStaticConfig.MaintenanceIntervals();
                        if (intervals.Count > 0)
                        {
                            for (int interval = 0; interval < intervals.Count; interval++)
                            {
                                for (int x = 0; x < intervals[interval].Categories.Count; x++)
                                {
                                    if (intervals[interval].Categories[x] == item.MachineCategoryId)
                                    {
                                        newRecord.Interval = intervals[interval].Interval;
                                        newRecord.InitialMaintenance = intervals[interval].Initial;
                                        if (machineryHistorical.Hourmeter <= intervals[interval].Initial)
                                        {
                                            machineryHistorical.NextMaintenance = intervals[interval].Initial;
                                            machineryHistorical.MissingForNextMaintenance = machineryHistorical.NextMaintenance - machineryHistorical.Hourmeter;
                                        }
                                        else
                                        {
                                            decimal position = machineryHistorical.Hourmeter / intervals[interval].Interval;
                                            machineryHistorical.NextMaintenance = (Math.Truncate(position) * intervals[interval].Interval) + intervals[interval].Interval;
                                            machineryHistorical.MissingForNextMaintenance = machineryHistorical.NextMaintenance - machineryHistorical.Hourmeter;
                                        }
                                    }
                                }
                            }
                        }
                        var newMachine = await _machineryRepositoryAsync.AddAsync(newRecord);
                        await _machineryRepositoryAsync.SaveChangesAsync();

                        machineryHistorical.MachineryId = newMachine.Id;
                        machineryHistorical.CreationDate = DateTime.Now;
                        machineryHistorical.IsSystemAlert = false;
                        await _historicalRepositoryAsync.AddAsync(machineryHistorical);
                        await _historicalRepositoryAsync.SaveChangesAsync();

                        var failure = new MachineryFailureReport();
                        failure.CreationDate = DateTime.Now;
                        failure.CreatedBy = "Sistema";
                        failure.StatusId = getStatus.Id;
                        failure.MachineryFailureId = getFailure.Id;
                        failure.Description = "Sin falla";
                        failure.MachineryId = newMachine.Id;
                        await _reportFailureRepositoryAsync.AddAsync(failure);
                        await _reportFailureRepositoryAsync.SaveChangesAsync();
                    }
                    else
                    {
                        var machineryHistorical = new MachineryRootcloudHistorical();
                        machineryHistorical.Hourmeter = 0;
                        machineryHistorical.Lat = 0;
                        machineryHistorical.Lng = 0;
                        machineryHistorical.Status = "N/A";
                        machineryHistorical.Milenage = 0;
                        machineryHistorical.FuelLevel = 0;
                        machineryHistorical.AverageFuelConsumption = 0;
                        machineryHistorical.RealtimeFuelConsumption = 0;
                        machineryHistorical.TotalFuelConsumption = 0;
                        machineryHistorical.TotalFuelUnit = "N/A";
                        machineryHistorical.TimestampLocal = "N/A";

                        var intervals = RootcloudStaticConfig.MaintenanceIntervals();
                        if (intervals.Count > 0)
                        {
                            for (int interval = 0; interval < intervals.Count; interval++)
                            {
                                for (int x = 0; x < intervals[interval].Categories.Count; x++)
                                {
                                    if (intervals[interval].Categories[x] == item.MachineCategoryId)
                                    {
                                        newRecord.Interval = intervals[interval].Interval;
                                        newRecord.InitialMaintenance = intervals[interval].Initial;
                                        if (machineryHistorical.Hourmeter <= intervals[interval].Initial)
                                        {
                                            machineryHistorical.NextMaintenance = intervals[interval].Initial;
                                            machineryHistorical.MissingForNextMaintenance = machineryHistorical.NextMaintenance - machineryHistorical.Hourmeter;
                                        }
                                        else
                                        {
                                            decimal position = machineryHistorical.Hourmeter / intervals[interval].Interval;
                                            machineryHistorical.NextMaintenance = (Math.Truncate(position) * intervals[interval].Interval) + intervals[interval].Interval;
                                            machineryHistorical.MissingForNextMaintenance = machineryHistorical.NextMaintenance - machineryHistorical.Hourmeter;
                                        }
                                    }
                                }
                            }
                        }

                        var newMachine = await _machineryRepositoryAsync.AddAsync(newRecord);
                        await _machineryRepositoryAsync.SaveChangesAsync();

                        machineryHistorical.MachineryId = newMachine.Id;
                        machineryHistorical.CreationDate = DateTime.Now;
                        machineryHistorical.IsSystemAlert = false;
                        await _historicalRepositoryAsync.AddAsync(machineryHistorical);
                        await _historicalRepositoryAsync.SaveChangesAsync();

                        var failure = new MachineryFailureReport();
                        failure.CreationDate = DateTime.Now;
                        failure.CreatedBy = "Sistema";
                        failure.StatusId = getStatus.Id;
                        failure.MachineryFailureId = getFailure.Id;
                        failure.Description = "Sin falla";
                        failure.MachineryId = newMachine.Id;
                        await _reportFailureRepositoryAsync.AddAsync(failure);
                        await _reportFailureRepositoryAsync.SaveChangesAsync();
                    }
                }
                else
                {
                    machinery.BaseInfoId = item.BaseInfoId;
                    machinery.CatName = item.CatName;
                    machinery.DeviceName = item.DeviceName;
                    machinery.Customer = item.Customer == null ? "Sin Asignar" : item.Customer;
                    machinery.Country = item.Country;
                    machinery.MachineCategoryId = item.MachineCategoryId;
                    machinery.MachineTypeId = item.MachineTypeId;
                    machinery.MachineTypeName = item.MachineTypeName;
                    machinery.ModelType = item.ModelType;
                    machinery.TenantId = item.TenantId;
                    machinery.IsRootcloudActive = true;

                    await _machineryRepositoryAsync.UpdateAsync(machinery);
                    await _machineryRepositoryAsync.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<ResponseDto<List<WorkingConditionDto>>> WorkingConditions()
        {
            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                await WorkingConditions();

            var url = $"{RootcloudStaticConfig.baseUri}/service-model-manager/settingAlarm/queryFilterListForUser";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("language", "es-ES");
            request.AddHeader("time_zone", 8);
            request.AddQueryParameter("departmentId", session.OrgIds);
            request.AddHeader("access-token", session.TicketId);
            var body = new WorkingConditionRequestDto()
            {
                TenantId = session.TenantId.ToString(),
                DepartmentId = session.OrgIds,
                OrgId = session.OrgIds,
                UserId = session.UserId,
                Source = 1
            };
            var json = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            HttpStatusCode statuscode = response.StatusCode;
            int numericStatusCode = (int)statuscode;
            if (numericStatusCode == 200 && response.Content != null)
            {
                var result = JsonConvert.DeserializeObject<ResponseDto<List<WorkingConditionDto>>>(response.Content.ToString());
                if (result == null)
                    throw new ApiException("Rootcloud error: la respuesta a esta solicitud no pudo ser mapeada");
                if (result.Msg!.ToLower().Contains("error")
                    || result.Msg.ToLower().Contains("ApiException"))
                {
                    throw new ApiException(result.Msg);
                }
                return result;
            }
            else
            {
                throw new ApiException("Rootcloud error: Ocurrio un error al obtener esta informacion");
            }
        }
    }
}