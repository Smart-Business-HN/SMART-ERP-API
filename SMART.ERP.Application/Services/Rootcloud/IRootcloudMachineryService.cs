

using SMART.ERP.Application.DTOs.Rootcloud;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public interface IRootcloudMachineryService
    {
        Task<ResponseDto<List<WorkingConditionDto>>> WorkingConditions();
        Task<List<DeviceDto>> GetAllMachineries();
        Task<bool> CreateOrUpdateMachinery(List<DeviceDto> devices);
    }
}
