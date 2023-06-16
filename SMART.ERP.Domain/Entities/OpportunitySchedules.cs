namespace SMART.ERP.Domain.Entities
{
    public class OpportunitySchedules
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int OpportunityAge { get; set; }
    }
}
