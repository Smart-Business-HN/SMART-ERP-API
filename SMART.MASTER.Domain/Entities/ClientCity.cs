namespace SMART.MASTER.Domain.Entities
{
    public class ClientCity
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
