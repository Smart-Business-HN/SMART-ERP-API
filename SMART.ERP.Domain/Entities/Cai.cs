using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class Cai
    {
        public int Id { get; init; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        public int? BranchOfficeId { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        [MaxLength(11)]
        public string Prefix { get; set; } = null!;
        [MaxLength(37)]
        public string Identificator { get; set; } = null!;
        public int StartCorrelative { get; set; }
        public int EndCorrelative { get; set; }
        public int CurrentCorrelative { get; set; }
        public int AvailableInvoices { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsGeneralCai { get; set; }
        public bool IsActive { get; set; }
    }
}
