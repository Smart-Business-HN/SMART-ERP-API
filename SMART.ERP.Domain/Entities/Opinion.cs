namespace SMART.ERP.Domain.Entities
{
    public class Opinion
    {
        public int Id { get; init; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public string Observation { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string Charge { get; set; } = null!;
        public bool IsActive { get; set; }
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
    }
}
