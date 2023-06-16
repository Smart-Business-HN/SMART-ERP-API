namespace SMART.ERP.Domain.Entities
{
    public class AdvisorGoal
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public decimal Goal { get; set; }
        public DateTime InitDate { get; set; }
        public bool Enabled { get; set; }
        public DateTime EndDate { get; set; }
    }
}
