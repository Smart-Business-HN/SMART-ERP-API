using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryResumeDto
    {
        public int Id { get; set; }
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
        public int MachineCategoryId { get; set; }
        public decimal Interval { get; set; }
        public decimal InitialMaintenance { get; set; }
        public bool IsRootcloudActive { get; set; }
        public bool ActiveMaintenance { get; set; }
        public int? SubcategoryId { get; set; }
        public string? Status { get; set; }
        public int? BrandId { get; set; }
        public BrandDto? Brand { get; set; }
        public SubcategoryDto? Subcategory { get; set; }
        public List<MachineryRootcloudHistoricalDto>? MachineyRootcloudHistoricals { get; set; }
        public List<MachineryMaintenanceDto>? MachineryMaintenances { get; set; }
        public List<MachineryFailureReportDto>? MachineryFailureReports { get; set; }
    }
}
