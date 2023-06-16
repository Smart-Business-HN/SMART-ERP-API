namespace SMART.ERP.Domain.Entities
{
    public class Department
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<City>? Cities { get; set; } = new List<City>();
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int? RegionId { get; set; }
        public virtual Region? Region { get; set; }
        public List<AdvisorDepartment>? Users { get; set; } = new List<AdvisorDepartment>();
    }
}
