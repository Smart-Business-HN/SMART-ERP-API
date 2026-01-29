using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class SocialReason
    {
        public int Id { get; init; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
