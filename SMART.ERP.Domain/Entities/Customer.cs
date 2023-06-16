namespace SMART.ERP.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; init; }
        public Guid MasterId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
        public List<CustomerMachinery>? CustomerMachinery { get; set; }
    }
}
