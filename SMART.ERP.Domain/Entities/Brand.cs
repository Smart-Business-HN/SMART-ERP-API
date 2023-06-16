namespace SMART.ERP.Domain.Entities
{
    public class Brand
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public string? Background { get; set; }
        public bool IsActive { get; set; }
    }
}
