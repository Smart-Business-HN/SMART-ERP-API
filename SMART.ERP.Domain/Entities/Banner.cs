namespace SMART.ERP.Domain.Entities
{
    public class Banner
    {
        public int Id { get; init; }
        public string Url { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public bool IsActive { get; set; }
    }
}
