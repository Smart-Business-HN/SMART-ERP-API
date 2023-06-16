namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class DeviceDto
    {
        public string BaseInfoId { get; set; } = null!;
        public string DeviceName { get; set; } = null!;
        public string SerialNum { get; set; } = null!;
        public string ModelType { get; set; } = null!;
        public string MachineTypeId { get; set; } = null!;
        public string MachineTypeName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string CreateDate { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string? Customer { get; set; }
        public string CatName { get; set; } = null!;
        public int MachineCategoryId { get; set; }
        public decimal Hourmeter { get; set; }
        public decimal Milenage { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string? Status { get; set; }
        public decimal NextMaintenance { get; set; }
        public decimal MissingForNextMaintenance { get; set; }
        public string TimestampLocal { get; set; } = null!;
        public decimal LateHours { get; set; }
        public decimal LastMaintenance { get; set; }
        public bool MaintenanceDone { get; set; }
        public decimal AverageFuelConsumption { get; set; }
        public decimal RealtimeFuelConsumption { get; set; }
        public decimal FuelLevel { get; set; }
        public decimal TotalFuelConsumption { get; set; }
        public string? TotalFuelUnit { get; set; }
        public bool SystemRunning { get; set; }
    }
}
