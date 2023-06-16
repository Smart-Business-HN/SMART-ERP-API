using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class MachineryFailureReportDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public int MachineryFailureId { get; set; }
        public MachineryFailureDto? MachineryFailure { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public int MachineryId { get; set; }
        public MachineryBasicDto? Machinery { get; set; }
    }
}
