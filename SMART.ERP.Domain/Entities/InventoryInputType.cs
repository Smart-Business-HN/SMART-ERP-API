namespace SMART.ERP.Domain.Entities
{
    public class InventoryInputType
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
