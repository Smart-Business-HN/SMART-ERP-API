namespace SMART.ERP.Application.DTOs.Product
{
    public class ProductDataSheetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int DataSheetId { get; set; }
        public DataSheetDto? DataSheet { get; set; }
        public bool IsActive { get; set; }
    }
}
