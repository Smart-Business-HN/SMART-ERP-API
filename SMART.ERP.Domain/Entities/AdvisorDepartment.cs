namespace SMART.ERP.Domain.Entities
{
    public class AdvisorDepartment
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
    }
}
