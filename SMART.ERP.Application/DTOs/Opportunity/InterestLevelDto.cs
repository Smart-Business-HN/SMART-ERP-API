namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class InterestLevelDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
