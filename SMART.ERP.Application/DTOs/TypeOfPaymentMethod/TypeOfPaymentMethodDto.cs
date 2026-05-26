namespace SMART.ERP.Application.DTOs.TypeOfPaymentMethod
{
    public class TypeOfPaymentMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool RequiresBankAccount { get; set; }
    }
}
