namespace SMART.ERP.Domain.Entities
{
    public class ProductFeature
    {
        public int Id { get; init; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
