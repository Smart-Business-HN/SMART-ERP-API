namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class WorkingConditionDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int DeviceModelId { get; set; }
        public int ApplicationId { get; set; }
        public string? FieldName { get; set; }
        public string? FieldAlias { get; set; }
        public string? DisplayName { get; set; }
        public string? FieldType { get; set; }
        public string? TransformationUnit { get; set; }
    }
}
