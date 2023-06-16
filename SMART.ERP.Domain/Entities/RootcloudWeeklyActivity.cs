namespace SMART.ERP.Domain.Entities
{
    public class RootcloudWeeklyActivity
    {
        public int Id { get; init; }
        //public string Year { get; set; } = null!;
        //public string Month { get; set; } = null!;
        //public string Week { get; set; } = null!;
        //public string Day { get; set; } = null!;
        public List<MachineryRootcloudHistorical>? RootcloudDeviceHistoricals { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
