namespace SMART.ERP.Domain.Entities
{
    public class Warehouse
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public bool IsGeneralWarehouse { get; set; }
        public int? CityId { get; set; }
        public virtual City? City { get; set;}
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
    }
}
