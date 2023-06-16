namespace SMART.ERP.Application.DTOs.Product
{
    public class CreateProductDataSheetDto
    {
        public string Title { get; set; } = null!;
        public int DataSheetId { get; set; }
        public bool IsActive { get; set; }
    }
}
