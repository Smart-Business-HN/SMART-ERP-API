namespace SMART.ERP.Application.DTOs.Company
{
    public class BannerDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
