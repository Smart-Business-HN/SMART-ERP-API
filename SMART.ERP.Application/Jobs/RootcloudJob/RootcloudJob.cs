using Quartz;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Jobs.RootcloudJob
{
    public class RootcloudJob : IJob
    {
        public static readonly JobKey Key = new JobKey("rootcloud-job", "remote-jobs");
        private readonly IRootcloudHistoricalService _rootcloudService;
        public RootcloudJob(IRootcloudHistoricalService rootcloudService)
        {
            _rootcloudService = rootcloudService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _rootcloudService.RootcloudHistoricalJob();
        }
    }
}
