using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class ProjectAttachmentCategory
    {
        public int Id { get; init; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
