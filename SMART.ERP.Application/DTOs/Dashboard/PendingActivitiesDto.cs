namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class PendingActivitiesDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public int NumPendingActivities { get; set; }
        public int NumFinishedActivities { get; set; }
        public int NumPausedActivities { get; set; }
        public int Total { get; set; }
        public decimal CompletionPercentage { get; set; }
    }
}
