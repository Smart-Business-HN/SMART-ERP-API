using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Services.Rootcloud
{
    public interface IRootcloudSessionService
    {
        Task<ResponseDto<LoginResponseDto>> Login();
        Task<RootcloudSession> CheckAndUpdateSession();
        Task RemoveSession();
    }
}
