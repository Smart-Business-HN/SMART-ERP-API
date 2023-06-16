namespace SMART.ERP.Domain.Entities
{
    public class Role
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string Selector { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
