namespace SMART.ERP.Domain.Entities
{
    public class Machinery
    {
        public int Id { get; init; }
        public string BaseInfoId { get; set; } = null!;
        public string DeviceName { get; set; } = null!;
        public string SerialNum { get; set; } = null!;
        public string ModelType { get; set; } = null!;
        public string MachineTypeId { get; set; } = null!;
        public string MachineTypeName { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string TenantId { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string CatName { get; set; } = null!;
        public string? Status { get; set; }
        public int MachineCategoryId { get; set; }
        public decimal Interval { get; set; }
        public decimal InitialMaintenance { get; set; }
        public bool IsRootcloudActive { get; set; }
        public bool ActiveMaintenance { get; set; }
        public int? SubcategoryId { get; set; }
        public virtual Subcategory? Subcategory { get; set; }
        public int? BrandId { get; set; }
        public virtual Brand? Brand { get; set; }
        public List<MachineryRootcloudHistorical> MachineyRootcloudHistoricals { get; set; } = new List<MachineryRootcloudHistorical>();
        public List<MachineryMaintenance> MachineryMaintenances { get; set; } = new List<MachineryMaintenance>();
        public List<MachineryFailureReport> MachineryFailureReports { get; set; } = new List<MachineryFailureReport>();
    }
}
