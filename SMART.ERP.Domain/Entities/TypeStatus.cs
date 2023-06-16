namespace SMART.ERP.Domain.Entities
{
    public class TypeStatus
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public virtual List<Status>? Status { get; set; }
    }
}
