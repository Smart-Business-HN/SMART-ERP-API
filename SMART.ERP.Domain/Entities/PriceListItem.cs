using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class PriceListItem
    {
        public int Id { get; init; }
        public int PriceListId { get; set; }
        public virtual PriceList? PriceList { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public DateTime CreationDate { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        [MaxLength(50)]
        public string? ModificatedBy { get; set; }
    }
}
