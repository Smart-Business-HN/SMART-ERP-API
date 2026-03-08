namespace SMART.ERP.Application.DTOs.Quotation
{
    public class FieldChangeDto
    {
        public string Field { get; set; } = null!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
