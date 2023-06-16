namespace SMART.ERP.Domain.Entities
{
    public class InterestLevel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
