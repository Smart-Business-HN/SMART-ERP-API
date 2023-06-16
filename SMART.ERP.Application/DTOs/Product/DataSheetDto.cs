namespace SMART.ERP.Application.DTOs.Product
{
    public class DataSheetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool? IsOutstanding { get; set; }
    }
}
