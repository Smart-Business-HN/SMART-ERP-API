namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class LossReasonDto
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
