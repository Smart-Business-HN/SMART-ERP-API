namespace SMART.ERP.Domain.Entities
{
    public class Subcategory
    {
        public int Id { get; init; }
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
    }
}
