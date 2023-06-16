namespace SMART.ERP.Domain.Entities
{
    public class ProductPart
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int FatherProductId { get; set; }
        public virtual Product FatherProduct { get; set; } = null!;
        public string FatherProductCode { get; set; } = null!;
        public string FatherProductName { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
