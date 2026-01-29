using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class Tax
    {
        public int Id { get; init; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Precision(18, 0)]
        public decimal Rate { get; set; }
        [MaxLength(50)]
        public string TextForDocuments { get; set; } = null!;
    }
}
