using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Domain.Entities;
using System.Net;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public class RootcloudHistoricalService : IRootcloudHistoricalService
    {
        private readonly IRootcloudSessionService _rootcloudSessionService;
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _repositoryAsync;
        private readonly IRepositoryAsync<Machinery> _machineryRepositoryAsync;
        private readonly IMailService _mailService;

        public RootcloudHistoricalService(IRootcloudSessionService rootcloudSessionService,
            IRepositoryAsync<MachineryRootcloudHistorical> repositoryAsync,
            IRepositoryAsync<Machinery> machineryRepositoryAsync,
            IMailService mailService)
        {
            _rootcloudSessionService = rootcloudSessionService;
            _repositoryAsync = repositoryAsync;
            _machineryRepositoryAsync = machineryRepositoryAsync;
            _mailService = mailService;
        }

        public async Task RootcloudHistoricalJob()
        {
            await _rootcloudSessionService.RemoveSession();
            await _rootcloudSessionService.Login();


            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                await RootcloudHistoricalJob();

            bool isUpdate = false;
            List<AlertSystemNotRunningDto> listAlert = new List<AlertSystemNotRunningDto>();
            List<MachineryRootcloudHistorical> historicals = new List<MachineryRootcloudHistorical>();
            var machineries = await _machineryRepositoryAsync.ListAsync(new FilterMachineryByHistoricalSpecification());
            if (machineries.Count > 0)
            {
                foreach (var item in machineries)
                {
                    if (item.BaseInfoId != "N/A")
                    {
                        if (item.MachineyRootcloudHistoricals.Where(w => w.CreationDate.Date == DateTime.UtcNow.Date).ToList().Count > 0)
                        {
                            isUpdate = true;
                            var reult = await UpdateMachineryRootcloudHistorical(
                                item.MachineyRootcloudHistoricals.Last(), item, session);
                            if (reult.NextSystemAlert.HasValue)
                            {
                                if (reult.NextSystemAlert.Value.Date == DateTime.UtcNow.Date)
                                {
                                    var alert = new AlertSystemNotRunningDto()
                                    {
                                        SerialNum = item.SerialNum,
                                        Hourmeter = reult.Hourmeter,
                                        TimestampLocal = reult.TimestampLocal
                                    };
                                    listAlert.Add(alert);
                                }
                            }

                            historicals.Add(reult);
                        }
                        else
                        {
                            var response = await CreateMachineryRootcloudHistorical(item, item.MachineyRootcloudHistoricals.Last(), session);
                            historicals.Add(response);
                        }
                    }
                }

                if (isUpdate)
                {
                    if (historicals.Count > 0)
                    {
                        historicals = historicals.Where(x => x.Hourmeter != 0 || x.TimestampLocal != null).ToList();
                        await _repositoryAsync.UpdateRangeAsync(historicals);
                        await _repositoryAsync.SaveChangesAsync();
                    }
                }
                else
                {
                    if (historicals.Count > 0)
                    {
                        historicals = historicals.Where(x => x.Hourmeter != 0 || x.TimestampLocal != null).ToList();
                        await _repositoryAsync.AddRangeAsync(historicals);
                        await _repositoryAsync.SaveChangesAsync();
                    }
                }
            }


            if (listAlert.Count > 0)
            {
                MailRequestDto mail = new MailRequestDto();
                mail.Subject = "Equipos sin transmitir datos";
                mail.ToEmail = "dtabora@grupoplatino.hn";
                //mail.ToCCEmail = "jflores@platino.hn";
                mail.Body = @"<table style=""width:100%;border-collapse:collapse;border:1px solid black;""><tr><th>Serie</th><th>Horometro</th><th>Ult. Transmision</th></tr>";
                foreach (var alert in listAlert)
                {
                    mail.Body += $"<tr><td style=\"text-align:center;border:1px solid black;\">{alert.SerialNum}</td>" +
                        $"<td style=\"text-align:center;border:1px solid black;\">{alert.Hourmeter}</td>" +
                        $"<td style=\"text-align:center;border:1px solid black;\">{DateTime.Parse(alert.TimestampLocal).ToString("d")}</td></tr>";
                }

                mail.Body += "</table>";

                await _mailService.SendEmailAsync(mail);
            }
        }

        public async Task<ResponseDto<List<WorkingItemDto>>> HistoricalWorkingConditions(string baseInfoId, string modelType,
            DateTime startTime, DateTime endTime, int pageIndex, int pageSize, RootcloudSession session)
        {
            var url = $"{RootcloudStaticConfig.baseUri}/report-management/alarmDuty/engineerList";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("language", "es-ES");
            request.AddHeader("time_zone", 8);
            request.AddHeader("access-token", session.TicketId);
            var body = new WorkingHistoricalRequestDto()
            {
                BaseInfoId = long.Parse(baseInfoId),
                DeviceModelId = long.Parse(modelType),
                StartTime = startTime.ToString("u"),
                EndTime = endTime.ToString("u"),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TenantId = session.TenantId.ToString(),
                Dimentions = RootcloudStaticConfig.GetFieldNames()
            };
            var json = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            HttpStatusCode statuscode = response.StatusCode;
            int numericStatusCode = (int)statuscode;
            if (response.Content != null && numericStatusCode == 200)
            {
                var result = JsonConvert.DeserializeObject<ResponseDto<List<WorkingItemDto>>>(response.Content.ToString());
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

        public async Task<MachineryRootcloudHistorical> CreateMachineryRootcloudHistorical(Machinery machinery,
            MachineryRootcloudHistorical rootcloudHistorical, RootcloudSession session)
        {
            var machineryHistorical = new MachineryRootcloudHistorical();
            machineryHistorical.NextMaintenance = rootcloudHistorical.NextMaintenance;
            machineryHistorical.MissingForNextMaintenance = rootcloudHistorical.MissingForNextMaintenance;
            machineryHistorical.LastMaintenance = rootcloudHistorical.LastMaintenance;
            machineryHistorical = await CalculateNextMaintenanceAndAssignData(machineryHistorical, machinery, session);

            machineryHistorical.MachineryId = machinery.Id;
            machineryHistorical.CreationDate = DateTime.UtcNow;

            return machineryHistorical;
        }


        public async Task<MachineryRootcloudHistorical> UpdateMachineryRootcloudHistorical(
            MachineryRootcloudHistorical machineryHistorical, Machinery machinery, RootcloudSession session)
        {
            machineryHistorical = await CalculateNextMaintenanceAndAssignData(machineryHistorical, machinery, session);
            return machineryHistorical;
        }

        public async Task<MachineryRootcloudHistorical> CalculateNextMaintenanceAndAssignData(MachineryRootcloudHistorical rootcloudHistorical, Machinery machinery, RootcloudSession session)
        {
            var historical = await HistoricalWorkingConditions(machinery.BaseInfoId, machinery.ModelType, DateTime.Now.AddDays(-200), DateTime.Now, 1, 10, session);
            if (historical.Data == null)
            {
                throw new ApiException($"No se encontro datos de Rootcloud para la maquina con numero de serie: {machinery.SerialNum}");
            }

            if (historical.Data.Count > 0)
            {
                var selectFirst = historical.Data.First();
                rootcloudHistorical.Lat = selectFirst.GPS_Latitude;
                rootcloudHistorical.Lng = selectFirst.GPS_Longitude;
                rootcloudHistorical.Status = selectFirst.DeviceStatus;
                rootcloudHistorical.Hourmeter = RootcloudStaticConfig.WorkingHour(selectFirst);
                rootcloudHistorical.Milenage = selectFirst.Mileage > 0 ? selectFirst.Mileage
                    : selectFirst.Total_mileage > 0 ? selectFirst.Total_mileage : 0;
                rootcloudHistorical.TimestampLocal = selectFirst.Timestamp_Local == null ? DateTime.Now.ToString() : selectFirst.Timestamp_Local;
                rootcloudHistorical.FuelLevel = selectFirst.Fuel_level;
                rootcloudHistorical.AverageFuelConsumption = selectFirst.Average_fuel_consumption;
                rootcloudHistorical.RealtimeFuelConsumption = selectFirst.Realtime_fuel_consumption;
                rootcloudHistorical.TotalFuelConsumption = selectFirst.Total_fuel_consumption;

                DateTime checkEndDate = DateTime.Parse(selectFirst.Timestamp_Local);
                if (checkEndDate.Date < DateTime.Now.Date)
                {
                    double diffOfDates = (DateTime.Now.Date - checkEndDate.Date).TotalDays;
                    if (diffOfDates > 6)
                    {
                        if (rootcloudHistorical.NextSystemAlert.HasValue)
                        {
                            if (DateTime.Now.Date > rootcloudHistorical.NextSystemAlert.Value.Date)
                            {
                                rootcloudHistorical.IsSystemAlert = true;
                                rootcloudHistorical.NextSystemAlert = DateTime.Now.AddDays(6);
                            }
                        }
                        else
                        {
                            rootcloudHistorical.IsSystemAlert = true;
                            rootcloudHistorical.NextSystemAlert = DateTime.Now.AddDays(6);
                        }
                        rootcloudHistorical.SystemRunning = false;
                    }
                }
                else
                {
                    rootcloudHistorical.SystemRunning = true;
                }

                rootcloudHistorical = AssingNextMaintenance(rootcloudHistorical, machinery);
            }

            return rootcloudHistorical;
        }

        public MachineryRootcloudHistorical AssingNextMaintenance(MachineryRootcloudHistorical rootcloudHistorical, Machinery machinery)
        {
            if (rootcloudHistorical.NextMaintenance == 0 && machinery.MachineyRootcloudHistoricals.Count > 0)
            {
                rootcloudHistorical.NextMaintenance = machinery.MachineyRootcloudHistoricals.First().NextMaintenance;
            }

            if (machinery.MachineyRootcloudHistoricals.Count > 0)
            {
                rootcloudHistorical.MaintenanceDone = machinery.MachineyRootcloudHistoricals.First().MaintenanceDone;
                if (rootcloudHistorical.LastMaintenance < machinery.MachineyRootcloudHistoricals.First().LastMaintenance)
                    rootcloudHistorical.LastMaintenance = machinery.MachineyRootcloudHistoricals.First().LastMaintenance;

                if (machinery.MachineyRootcloudHistoricals.First().LastMaintenance > rootcloudHistorical.Hourmeter)
                    rootcloudHistorical.Hourmeter = machinery.MachineyRootcloudHistoricals.First().LastMaintenance;
            }

            if (rootcloudHistorical.Hourmeter > rootcloudHistorical.NextMaintenance)
            {
                rootcloudHistorical.MaintenanceDone = false;
            }

            if (rootcloudHistorical.LastMaintenance == 0
                && rootcloudHistorical.NextMaintenance != machinery.InitialMaintenance
                && rootcloudHistorical.Hourmeter < rootcloudHistorical.NextMaintenance)
            {
                decimal position = rootcloudHistorical.Hourmeter / machinery.Interval;
                rootcloudHistorical.NextMaintenance = Math.Truncate(position) * machinery.Interval + machinery.Interval;
                rootcloudHistorical.MissingForNextMaintenance = rootcloudHistorical.NextMaintenance - rootcloudHistorical.Hourmeter;
                rootcloudHistorical.LateHours = 0;
            }
            else if (rootcloudHistorical.Hourmeter <= machinery.InitialMaintenance && !rootcloudHistorical.MaintenanceDone)
            {
                rootcloudHistorical.NextMaintenance = machinery.InitialMaintenance;
                rootcloudHistorical.MissingForNextMaintenance = rootcloudHistorical.NextMaintenance - rootcloudHistorical.Hourmeter;
            }
            else if (!rootcloudHistorical.MaintenanceDone
                && rootcloudHistorical.Hourmeter > rootcloudHistorical.NextMaintenance
                && rootcloudHistorical.NextMaintenance > 0)
            {
                rootcloudHistorical.LateHours = rootcloudHistorical.NextMaintenance - rootcloudHistorical.Hourmeter;
                rootcloudHistorical.MissingForNextMaintenance = 0;
            }
            else if (rootcloudHistorical.MaintenanceDone
                && rootcloudHistorical.Hourmeter < rootcloudHistorical.NextMaintenance
                && rootcloudHistorical.NextMaintenance > 0)
            {
                rootcloudHistorical.NextMaintenance = rootcloudHistorical.LastMaintenance + machinery.Interval;
                rootcloudHistorical.MissingForNextMaintenance = rootcloudHistorical.NextMaintenance - rootcloudHistorical.Hourmeter;
            }
            else if (!rootcloudHistorical.MaintenanceDone
                && rootcloudHistorical.Hourmeter < rootcloudHistorical.NextMaintenance
                && rootcloudHistorical.NextMaintenance > 0)
            {
                rootcloudHistorical.NextMaintenance = rootcloudHistorical.LastMaintenance + machinery.Interval;
                rootcloudHistorical.MissingForNextMaintenance = rootcloudHistorical.NextMaintenance - rootcloudHistorical.Hourmeter;
            }

            if (rootcloudHistorical.Hourmeter < rootcloudHistorical.NextMaintenance)
            {
                rootcloudHistorical.MaintenanceDone = false;
            }

            return rootcloudHistorical;
        }

        public async Task<MapGeolocationDto> GetLocationByLatAndLng(string lat, string lng, string serialNum)
        {
            var url = $"https://nominatim.openstreetmap.org/search?q={lat},{lng}&format=json";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            RestResponse response = await client.ExecuteAsync(request);
            HttpStatusCode statuscode = response.StatusCode;
            int numericStatusCode = (int)statuscode;
            if (response.Content != null && numericStatusCode == 200)
            {
                var result = JsonConvert.DeserializeObject<List<MapGeolocationDto>>(response.Content.ToString());
                if (result == null)
                    throw new ApiException($"No se encontro la ubicacion de la maquina {serialNum}");

                return result.First();
            }
            else
            {
                throw new ApiException("Rootcloud error: Ocurrio un error al obtener esta informacion");
            }
        }

        public async Task UpdateLocationJob()
        {
            await _rootcloudSessionService.RemoveSession();
            var session = await _rootcloudSessionService.CheckAndUpdateSession();
            if (!session.IsActive)
                await UpdateLocationJob();

            List<Machinery> listMachineries = new List<Machinery>();
            var machineries = await _machineryRepositoryAsync.ListAsync(new FilterMachineryByHistoricalSpecification());
            if (machineries.Count > 0)
            {
                foreach (var item in machineries)
                {
                    if (item.BaseInfoId != "N/A")
                    {
                        if (item.MachineyRootcloudHistoricals.Count > 0)
                        {
                            var historical = item.MachineyRootcloudHistoricals.First();
                            if (item.IsRootcloudActive && historical.Lat != 0 && historical.Lng != 0)
                            {
                                var result = await GetLocationByLatAndLng(
                                historical.Lat.ToString(),
                                historical.Lng.ToString(),
                                item.SerialNum);
                                if (result != null)
                                {
                                    string[] position = result.Display_name.Split(", ").Where(item => !int.TryParse(item, out int _)).ToArray();
                                    item.Country = position.Last();
                                    item.Province = position.ElementAt(position.Length - 2);

                                    listMachineries.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            await _machineryRepositoryAsync.UpdateRangeAsync(listMachineries);
            await _machineryRepositoryAsync.SaveChangesAsync();
        }
    }
}
