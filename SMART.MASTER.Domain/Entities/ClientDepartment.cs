namespace SMART.MASTER.Domain.Entities
{
    public class ClientDepartment
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<ClientCity>? Cities { get; set; }
    }
}
