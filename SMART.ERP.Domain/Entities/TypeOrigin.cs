namespace SMART.ERP.Domain.Entities
{
    public class TypeOrigin
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsProspectOrigin { get; set; }
    }
}
