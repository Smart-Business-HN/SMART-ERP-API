namespace SMART.ERP.Domain.Entities
{
    public class Status
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public int TypeStatusId { get; set; }
        public TypeStatus? TypeStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
