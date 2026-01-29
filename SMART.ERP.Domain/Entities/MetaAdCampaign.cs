using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class MetaAdCampaign
    {
        [Column(TypeName = "varchar(50)")]
        public string Id { get; set; } = null!;
        [Column(TypeName = "varchar(150)")]
        public string Name { get; set; } = null!;
        [Precision(18, 2)]
        public decimal Lifetime_Budget { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Stop_Time { get; set; }
    }
}
