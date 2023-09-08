namespace SMART.ERP.Domain.Entities
{
    public class Notification
    {
        public int Id { get; init; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Time { get; set; }
        public string? Link { get; set; }
        public bool UseRouter { get; set; }
        public bool Read { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
