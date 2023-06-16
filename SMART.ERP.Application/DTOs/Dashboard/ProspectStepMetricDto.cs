namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class ProspectStepMetricDto
    {
        public string Name { get; set; } = null!;
        public decimal Total { get; set; }
        public int NumProspects { get; set; }
    }
}
