namespace SMART.ERP.Application.DTOs.Rootcloud
{
    public class UnitOfMDto
    {
        public int ModelId { get; set; }
        public string? FieldAlias { get; set; }
        public string TransformationUnit { get; set; } = null!;
    }
}
