namespace SMART.ERP.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; init; }
        public string FileName { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
    }
}
