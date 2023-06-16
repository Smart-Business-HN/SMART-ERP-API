namespace SMART.MASTER.Domain.Entities
{
    public class ClientCountry
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public string CountryCode { get; set; } = null!;
        public List<ClientDepartment>? Departments { get; set; }
    }
}
