namespace SMART.ERP.Domain.Entities
{
    public class MachineryMaintenance
    {
        public int Id { get; init; }
        public string Person { get; set; } = null!;
        public string? UrlDocument { get; set; }
        public string? Observation { get; set; }
        public decimal Hourmeter { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public int MachineryId { get; set; }
        public virtual Machinery? Machinery { get; set; }
    }
}
