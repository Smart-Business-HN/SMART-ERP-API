namespace SMART.ERP.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;
        public string PhoneNumberCode { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<Region>? Regions { get; set; } = new List<Region>();
        public List<Department> Departments { get; set; } = new List<Department>();
    }
}
