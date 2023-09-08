namespace SMART.ERP.Domain.Entities
{
    public class CustomerType
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }

    }
}
