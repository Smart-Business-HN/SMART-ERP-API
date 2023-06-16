namespace SMART.ERP.Domain.Entities
{
    public class MachineryRootcloudHistorical
    {
        public int Id { get; init; }
        public decimal Hourmeter { get; set; }
        public decimal Milenage { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string? Status { get; set; }
        public decimal NextMaintenance { get; set; }
        public decimal LastMaintenance { get; set; }
        public decimal LateHours { get; set; }
        public bool MaintenanceDone { get; set; }
        public decimal MissingForNextMaintenance { get; set; }
        public string TimestampLocal { get; set; } = null!;
        public decimal AverageFuelConsumption { get; set; }
        public decimal RealtimeFuelConsumption { get; set; }
        public decimal FuelLevel { get; set; }
        public decimal TotalFuelConsumption { get; set; }
        public bool SystemRunning { get; set; }
        public string? TotalFuelUnit { get; set; }
        public bool IsSystemAlert { get; set; }
        public DateTime? NextSystemAlert { get; set; }
        public DateTime CreationDate { get; set; }
        public int MachineryId { get; set; }
        public virtual Machinery? Machinery { get; set; }
    }
}
