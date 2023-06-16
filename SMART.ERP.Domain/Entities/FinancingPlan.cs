namespace SMART.ERP.Domain.Entities
{
    public class FinancingPlan
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProviderId { get; set; }
        public virtual Provider? Provider { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
