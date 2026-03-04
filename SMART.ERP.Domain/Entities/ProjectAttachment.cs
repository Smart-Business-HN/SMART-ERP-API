using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class ProjectAttachment
    {
        public int Id { get; set; }
        [MaxLength(150)]
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        [MaxLength(100)]
        public string ContentType { get; set; } = null!;
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public int ProjectAttachmentCategoryId { get; set; }
        public virtual ProjectAttachmentCategory? ProjectAttachmentCategory { get; set; }
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        public DateTime CreationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string? ModificatedBy { get; set; }
    }
}
