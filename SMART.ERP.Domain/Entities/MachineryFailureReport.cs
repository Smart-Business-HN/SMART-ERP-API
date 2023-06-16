namespace SMART.ERP.Domain.Entities
{
    public class MachineryFailureReport
    {
        public int Id { get; init; }
        public string Description { get; set; } = null!;
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public int MachineryFailureId { get; set; }
        public virtual MachineryFailure? MachineryFailure { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public int MachineryId { get; set; }
        public virtual Machinery? Machinery { get; set; }
    }
}
