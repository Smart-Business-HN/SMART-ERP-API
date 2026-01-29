using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class AdvisorGoal
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        [Precision(18, 0)]
        public decimal Goal { get; set; }
        [Column(TypeName = "datetime2(0)")]
        public DateTime InitDate { get; set; }
        public bool Enabled { get; set; }
        [Column(TypeName = "datetime2(0)")]
        public DateTime EndDate { get; set; }
    }
}
