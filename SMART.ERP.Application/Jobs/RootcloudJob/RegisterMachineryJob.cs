using Quartz;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Jobs.RootcloudJob
{
    public class RegisterMachineryJob : IJob
    {
        public static readonly JobKey Key = new JobKey("register-machinery-job", "remote-jobs");
        private readonly IRootcloudMachineryService _rootcloudService;

        public RegisterMachineryJob(IRootcloudMachineryService rootcloudService)
        {
            _rootcloudService = rootcloudService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var devices = await _rootcloudService.GetAllMachineries();
            await _rootcloudService.CreateOrUpdateMachinery(devices);
        }
    }
}
