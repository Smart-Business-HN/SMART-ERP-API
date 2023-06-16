using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public interface IRootcloudHistoricalService
    {
        Task<ResponseDto<List<WorkingItemDto>>> HistoricalWorkingConditions(string baseInfoId, string modelType,
            DateTime startTime, DateTime endTime, int pageIndex, int pageSize, RootcloudSession session);
        Task<MachineryRootcloudHistorical> CreateMachineryRootcloudHistorical(Machinery machinery, MachineryRootcloudHistorical machineryRootcloudHistorical, RootcloudSession session);
        Task<MachineryRootcloudHistorical> UpdateMachineryRootcloudHistorical(
            MachineryRootcloudHistorical machineryRootcloudHistorical, Machinery machinery, RootcloudSession session);
        MachineryRootcloudHistorical AssingNextMaintenance(MachineryRootcloudHistorical rootcloudHistorical, Machinery machinery);
        Task RootcloudHistoricalJob();
        Task<MapGeolocationDto> GetLocationByLatAndLng(string lat, string lng, string serialNum);
        Task UpdateLocationJob();
    }
}
