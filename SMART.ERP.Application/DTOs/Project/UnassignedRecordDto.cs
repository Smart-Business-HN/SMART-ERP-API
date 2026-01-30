namespace SMART.ERP.Application.DTOs.Project
{
    public class UnassignedRecordDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Total { get; set; }
    }

    public class UnassignedRecordsResponseDto
    {
        public List<UnassignedRecordDto> Records { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
