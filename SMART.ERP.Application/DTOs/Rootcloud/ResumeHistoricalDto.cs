namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class ResumeHistoricalDto
    {
        public string? SerialNum { get; set; }
        public string? MachineTypeName { get; set; }
        public string? Customer { get; set; }
        public decimal InitHourmeter { get; set; }
        public decimal InitMilenage { get; set; }
        public decimal EndHourmeter { get; set; }
        public decimal EndMilenage { get; set; }
        public string Country { get; set; } = null!;
        public string Province { get; set; } = null!;
        public decimal NextMaintenance { get; set; }
        public decimal MissingForNextMaintenance { get; set; }
        public decimal AvarageHourmeter { get; set; }
        public decimal MissingDaysForNextMaintenance { get; set; }
    }
}
