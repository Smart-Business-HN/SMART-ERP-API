namespace SMART.ERP.Application.DTOs.Company
{
    public class OpinionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public string Observation { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string Charge { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
