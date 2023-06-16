namespace SMART.ERP.Application.DTOs.Address
{
    public class DepartmentDto
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? RegionId { get; set; }
        public int CountryId { get; set; }
        public IEnumerable<CityDto>? Cities { get; set; }
    }
}
