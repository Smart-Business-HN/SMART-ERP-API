namespace SMART.ERP.Application.DTOs.Address
{
    public class ClientCityDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
