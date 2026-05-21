using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class PriceList
    {
        public int Id { get; init; }
        [MaxLength(80)]
        public string Name { get; set; } = null!;
        [MaxLength(250)]
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
        public virtual List<PriceListItem>? Items { get; set; }
    }
}
