using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Notification
    {
        public int Id { get; init; }
        [Column(TypeName = "varchar(max)")]
        public string? Icon { get; set; }
        [Column(TypeName = "varchar(max)")]
        public string? Image { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? Title { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Time { get; set; }
        [Column(TypeName = "varchar(150)")]
        public string? Link { get; set; }
        public bool UseRouter { get; set; }
        public bool Read { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
