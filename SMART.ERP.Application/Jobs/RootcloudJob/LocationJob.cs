using Quartz;
using SMART.ERP.Application.Services.Rootcloud;

namespace SMART.ERP.Application.Jobs.RootcloudJob
{
    public class LocationJob : IJob
    {
        public static readonly JobKey Key = new JobKey("location-job", "locations-jobs");
        private readonly IRootcloudHistoricalService _rootcloudService;

        public LocationJob(IRootcloudHistoricalService rootcloudService)
        {
            _rootcloudService = rootcloudService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _rootcloudService.UpdateLocationJob();
        }
    }
}
