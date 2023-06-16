namespace SMART.ERP.Domain.Entities
{
    public class City
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public bool IsActive { get; set; }
    }
}
