namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class TypeOriginDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsProspectOrigin { get; set; }
    }
}
