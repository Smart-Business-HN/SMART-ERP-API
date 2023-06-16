namespace SMART.ERP.Application.DTOs.Product
{
    public class UnitOfMeasurementDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Abreviation { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
